# idol_vocal_analyzer/song_meta.py
import argparse
import csv
import json
from typing import Optional, List


def read_midis(path: str) -> List[float]:
    midis: List[float] = []
    with open(path, "r", encoding="utf-8") as f:
        r = csv.DictReader(f)
        if "midi" not in (r.fieldnames or []):
            raise ValueError("pitch.csv must contain 'midi' column")

        for row in r:
            m = row["midi"].strip()
            if m:
                try:
                    midis.append(float(m))
                except ValueError:
                    pass
    return midis


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--pitch_csv", required=True)
    p.add_argument("--out", default="song_meta.json")
    args = p.parse_args()

    midis = read_midis(args.pitch_csv)
    if not midis:
        meta = {"version": 1, "vocal_range_midi": None}
    else:
        meta = {
            "version": 1,
            "vocal_range_midi": {
                "min": float(min(midis)),
                "max": float(max(midis)),
            },
        }

    with open(args.out, "w", encoding="utf-8") as f:
        json.dump(meta, f, ensure_ascii=False, indent=2)

    print(f"Saved: {args.out}")


if __name__ == "__main__":
    main()