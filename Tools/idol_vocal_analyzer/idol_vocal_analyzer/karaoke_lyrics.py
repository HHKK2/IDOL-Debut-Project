from dataclasses import dataclass
from imghdr import tests
from typing import List, Dict, Any

from decorator import append


@dataclass
class LyricsLine:
    singer: str
    text: str
    start: float
    end: float

def build_karaoke_lines(lyrics_lines: List[LyricsLine], notes: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    result = []

    for i, line in enumerate(lyrics_lines):
        line_notes = [n for n in notes if line.start <= n["start"] < line.end]
        if not line_notes:
            continue
        text = line.text
        chars = [c for c in text if c != " "]
        total_note_dur = sum(n["end"] - n["start"] for n in line_notes)
        dur_per_char = total_note_dur / max(len(chars), 1)

        syllables = []
        cur_t = line_notes[0]["start"]
        for ch in chars:
            syllables.append({
                "ch": ch,
                "start": cur_t,
                "end": cur_t+dur_per_char,
            })
            cur_t += dur_per_char

        result.append({
            "line_index": i,
            "singer": line.singer,
            "text": text,
            "start": line_notes[0]["start"],
            "end": line_notes[-1]["end"],
            "syllables": syllables,
        })
    return result

import argparse, csv, json

def read_lyrics_csv(path: str) -> List[LyricsLine]:
    lines = []
    with open(path, "r", encoding="utf-8") as f:
        r = csv.DictReader(f)
        # 컬럼: start_sec, end_sec, singer, text
        for row in r:
            lines.append(
                LyricsLine(
                    singer=row["singer"],
                    text=row["text"],
                    start=float(row["start_sec"]),
                    end=float(row["end_sec"]),
                )
            )
    return lines

def main():
    p = argparse.ArgumentParser()
    p.add_argument("--lyrics_csv", required=True)
    p.add_argument("--chart_json", required=True)   # build_chart 가 만든 score_chart.json
    p.add_argument("--out", default="karaoke.json")
    args = p.parse_args()

    # 1) 가사 + 라인 타이밍 읽기
    lyrics_lines = read_lyrics_csv(args.lyrics_csv)

    # 2) 노트 읽기
    with open(args.chart_json, "r", encoding="utf-8") as f:
        chart = json.load(f)
    notes = chart["notes"]

    # 3) 가라오케 라인 생성
    lines = build_karaoke_lines(lyrics_lines, notes)

    with open(args.out, "w", encoding="utf-8") as f:
        json.dump({"lines": lines}, f, ensure_ascii=False, indent=2)

    print("Saved karaoke json:", args.out)

if __name__ == "__main__":
    main()