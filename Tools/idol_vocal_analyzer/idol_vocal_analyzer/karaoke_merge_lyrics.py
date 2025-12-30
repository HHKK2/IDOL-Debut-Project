from __future__ import annotations

import csv
import re
from dataclasses import dataclass
from pathlib import Path
from typing import List, Tuple, Dict


TAG_RE = re.compile(r"^\s*\[(P|M|ALL)\]\s*(.*)$", re.IGNORECASE)


@dataclass
class ParsedLine:
    singer: str   # "P" | "M" | "ALL"
    text: str


def tool_root() -> Path:
    # 현재 파일 위치에서 위로 올라가며 "data" 폴더가 있는 곳을 루트로 잡는다.
    here = Path(__file__).resolve()
    for p in [here.parent, *here.parents]:
        if (p / "data").exists():
            return p
    raise FileNotFoundError(f"Cannot find project root containing 'data' starting from: {here}")


def parse_lines_file(path: Path) -> List[ParsedLine]:
    out: List[ParsedLine] = []
    for raw in path.read_text(encoding="utf-8").splitlines():
        s = raw.strip()
        if not s:
            continue
        m = TAG_RE.match(s)
        if m:
            singer = m.group(1).upper()
            text = m.group(2).strip()
        else:
            singer = "P"
            text = s
        out.append(ParsedLine(singer=singer, text=text))
    return out


def read_start_csv(path: Path) -> List[Tuple[int, float]]:
    """
    지원:
    - comma CSV: line_index,start_sec
    - tab TSV:   line_index\tstart_sec
    - whitespace: "0 9.8"
    - header 유/무 모두
    """
    text = path.read_text(encoding="utf-8").splitlines()
    if not text:
        return []

    # 1) delimiter 추정: 첫 줄에 뭐가 더 많은지로 판단
    first = text[0]
    delim = ","
    if "\t" in first and first.count("\t") >= first.count(","):
        delim = "\t"

    def split_row(line: str) -> List[str]:
        line = line.strip()
        if not line:
            return []
        # 우선 delimiter로 split 해보고
        if delim in line:
            parts = [p.strip() for p in line.split(delim)]
        else:
            # fallback: 공백 여러개/탭 섞임까지 처리
            parts = re.split(r"\s+", line)
        return [p for p in parts if p != ""]

    rows = [split_row(l) for l in text]
    rows = [r for r in rows if len(r) > 0]
    if not rows:
        return []

    header = [h.strip().lower() for h in rows[0]]
    data_rows = rows[1:]

    # 헤더 판별: line_index 같은 문자열이 있으면 헤더로 본다
    has_header = any(h in ("line_index", "start_sec", "startsec", "time") for h in header)

    if not has_header:
        data_rows = rows
        # 2컬럼 기준: 0=line_index, 1=start_sec
        out = []
        for r in data_rows:
            if len(r) < 2:
                continue
            out.append((int(float(r[0])), float(r[1])))
        return out

    # 헤더가 있는 경우 컬럼 위치 찾기
    def norm(x: str) -> str:
        return re.sub(r"\s+", "", x.strip().lower())

    header_norm = [norm(h) for h in rows[0]]

    idx_line = None
    idx_start = None
    for i, h in enumerate(header_norm):
        if h in ("line_index", "lineindex", "idx"):
            idx_line = i
        if h in ("start_sec", "startsec", "start", "t", "time", "time_sec", "timesec"):
            idx_start = i

    # fallback: 그냥 앞 2개
    if idx_line is None or idx_start is None:
        idx_line, idx_start = 0, 1

    out: List[Tuple[int, float]] = []
    for r in data_rows:
        if len(r) <= max(idx_line, idx_start):
            continue
        li = r[idx_line].strip()
        st = r[idx_start].strip()
        if not li or not st:
            continue
        out.append((int(float(li)), float(st)))
    return out


def _looks_number(s: str) -> bool:
    try:
        float(s)
        return True
    except Exception:
        return False


def write_combined_csv(out_path: Path, lines: List[ParsedLine], starts: List[Tuple[int, float]]) -> None:
    # ✅ 폴더는 무조건 만들기
    out_path.parent.mkdir(parents=True, exist_ok=True)

    # ✅ 파일도 무조건 만들기(헤더라도)
    with out_path.open("w", newline="", encoding="utf-8") as f:
        w = csv.writer(f)
        w.writerow(["start_sec", "end_sec", "singer", "text"])

        starts_sorted = sorted(starts, key=lambda x: x[0])
        start_secs = [st for _, st in starts_sorted]

        if not start_secs:
            print(f"[WARN] start_sec 비어서 내용 없이 헤더만 생성됨: {out_path.resolve()}")
            return

        ends = start_secs[1:] + [start_secs[-1] + 3.0]

        time_map: Dict[int, Tuple[float, float]] = {}
        for (li, st), ed in zip(starts_sorted, ends):
            time_map[li] = (st, ed)

        wrote = 0
        for i, pl in enumerate(lines):
            if i not in time_map:
                continue
            st, ed = time_map[i]
            w.writerow([f"{st:.3f}", f"{ed:.3f}", pl.singer, pl.text])
            wrote += 1

    # ✅ 여기서 “진짜로 저장됐는지” 확인 로그
    print(f"[WRITE] {out_path.resolve()} (rows={wrote})")


def main():
    root = tool_root()
    lyrics_dir = root / "data" / "lyrics"
    out_dir = root / "data" / "lyrics_combined"

    print("[ROOT]", root.resolve())
    print("[LYRICS_DIR]", lyrics_dir.resolve())
    print("[OUT_DIR]", out_dir.resolve())

    if not lyrics_dir.exists():
        raise FileNotFoundError(f"lyrics dir not found: {lyrics_dir}")

    # *_lines.(txt|csv) / *_start.csv 를 곡 키 기준으로 매칭
    # 곡 키: F1_our_spring
    line_files = list(lyrics_dir.glob("*_lines.*"))
    start_files = list(lyrics_dir.glob("*_start.csv"))

    start_map = {p.stem.replace("_start", ""): p for p in start_files}

    made = 0
    for lp in line_files:
        song_key = lp.stem.replace("_lines", "")
        sp = start_map.get(song_key)
        if sp is None:
            continue

        lines = parse_lines_file(lp)
        starts = read_start_csv(sp)

        out_path = out_dir / f"{song_key}_lyrics.csv"
        write_combined_csv(out_path, lines, starts)
        print(f"[OK] {song_key} -> {out_path.resolve()}")
        made += 1

    print(f"DONE. combined={made}")
    print(song_key, "lines=", len(lines), "starts=", len(starts))


if __name__ == "__main__":
    main()