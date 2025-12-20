# idol_vocal_analyzer/vocal_segments.py
import argparse
import csv
from dataclasses import dataclass
from typing import List


@dataclass
class Segment:
    start: float
    end: float
    is_vocal: int  # 0 or 1


def read_segments_csv(path: str) -> List[Segment]:
    segs: List[Segment] = []
    with open(path, "r", encoding="utf-8") as f:
        r = csv.DictReader(f)
        required = {"start_sec", "end_sec", "is_vocal"}
        if set(r.fieldnames or []) != required:
            # allow extra cols but must contain required
            if not required.issubset(set(r.fieldnames or [])):
                raise ValueError(f"segments.csv must contain columns: {sorted(required)}")

        for row in r:
            segs.append(
                Segment(
                    start=float(row["start_sec"]),
                    end=float(row["end_sec"]),
                    is_vocal=int(row["is_vocal"]),
                )
            )
    return segs


def validate_and_normalize(segs: List[Segment]) -> List[Segment]:
    segs = sorted(segs, key=lambda s: (s.start, s.end))
    out: List[Segment] = []

    for s in segs:
        if s.end <= s.start:
            raise ValueError(f"Invalid segment (end<=start): {s}")
        if s.is_vocal not in (0, 1):
            raise ValueError(f"is_vocal must be 0 or 1: {s}")

        if not out:
            out.append(s)
            continue

        prev = out[-1]
        # 겹치면 에러 (A안에서는 겹치면 안 됨)
        if s.start < prev.end - 1e-9:
            raise ValueError(f"Overlapping segments: prev={prev} next={s}")

        # 딱 맞닿고 라벨 같으면 합치기
        if abs(s.start - prev.end) < 1e-6 and s.is_vocal == prev.is_vocal:
            out[-1] = Segment(prev.start, s.end, prev.is_vocal)
        else:
            out.append(s)

    return out


def write_segments_csv(segs: List[Segment], path: str) -> None:
    with open(path, "w", newline="", encoding="utf-8") as f:
        w = csv.writer(f)
        w.writerow(["start_sec", "end_sec", "is_vocal"])
        for s in segs:
            w.writerow([f"{s.start:.6f}", f"{s.end:.6f}", str(s.is_vocal)])


def main():
    p = argparse.ArgumentParser()
    p.add_argument("segments_csv", help="input segments.csv (start_sec,end_sec,is_vocal)")
    p.add_argument("--out", default="segments.norm.csv", help="output normalized segments csv")
    args = p.parse_args()

    segs = read_segments_csv(args.segments_csv)
    segs = validate_and_normalize(segs)
    write_segments_csv(segs, args.out)
    print(f"Saved: {args.out}  (segments={len(segs)})")


if __name__ == "__main__":
    main()