# idol_vocal_analyzer/build_chart.py
import argparse
import csv
import json
from dataclasses import dataclass
from typing import List, Optional, Dict, Any


@dataclass
class PitchRow:
    t: float
    f0_hz: Optional[float]
    midi: Optional[float]
    conf: float


@dataclass
class Segment:
    start: float
    end: float
    is_vocal: int


def read_pitch_csv(path: str) -> List[PitchRow]:
    rows: List[PitchRow] = []
    with open(path, "r", encoding="utf-8") as f:
        r = csv.DictReader(f)
        required = {"time_sec", "f0_hz", "midi", "confidence"}
        if not required.issubset(set(r.fieldnames or [])):
            raise ValueError(f"pitch.csv must contain columns: {sorted(required)}")

        for row in r:
            t = float(row["time_sec"])
            f0 = row["f0_hz"].strip()
            midi = row["midi"].strip()
            conf = float(row["confidence"]) if row["confidence"].strip() else 0.0

            rows.append(
                PitchRow(
                    t=t,
                    f0_hz=float(f0) if f0 else None,
                    midi=float(midi) if midi else None,
                    conf=conf,
                )
            )
    return rows


def read_segments_csv(path: str) -> List[Segment]:
    segs: List[Segment] = []
    with open(path, "r", encoding="utf-8") as f:
        r = csv.DictReader(f)
        required = {"start_sec", "end_sec", "is_vocal"}
        if not required.issubset(set(r.fieldnames or [])):
            raise ValueError(f"segments csv must contain columns: {sorted(required)}")

        for row in r:
            segs.append(
                Segment(
                    start=float(row["start_sec"]),
                    end=float(row["end_sec"]),
                    is_vocal=int(row["is_vocal"]),
                )
            )
    segs.sort(key=lambda s: s.start)
    return segs


def is_vocal_time(t: float, segs: List[Segment]) -> bool:
    # linear scan ok for now (files are small)
    for s in segs:
        if s.start <= t < s.end:
            return s.is_vocal == 1
    return False


def build_notes(
    pitch: List[PitchRow],
    segs: List[Segment],
    conf_threshold: float = 0.20,
    min_note_dur: float = 0.10,   # seconds
    tol_cents: int = 50,
) -> List[Dict[str, Any]]:
    """
    Output note events:
      { "start": float, "end": float, "midi": int, "tol_cents": int }
    """
    notes: List[Dict[str, Any]] = []
    cur: Optional[Dict[str, Any]] = None

    for pr in pitch:
        if not is_vocal_time(pr.t, segs):
            if cur is not None:
                notes.append(cur)
                cur = None
            continue

        if pr.midi is None or pr.conf < conf_threshold:
            if cur is not None:
                notes.append(cur)
                cur = None
            continue

        midi_q = int(round(pr.midi))

        if cur is None:
            cur = {"start": pr.t, "end": pr.t, "midi": midi_q, "tol_cents": tol_cents}
        else:
            if midi_q == cur["midi"]:
                cur["end"] = pr.t
            else:
                notes.append(cur)
                cur = {"start": pr.t, "end": pr.t, "midi": midi_q, "tol_cents": tol_cents}

    if cur is not None:
        notes.append(cur)

    # prune too short notes
    pruned: List[Dict[str, Any]] = []
    for n in notes:
        if (n["end"] - n["start"]) >= min_note_dur:
            pruned.append(n)

    return pruned


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--pitch_csv", required=True)
    p.add_argument("--segments_csv", required=True)
    p.add_argument("--out", default="score_chart.json")

    p.add_argument("--conf_th", type=float, default=0.20)
    p.add_argument("--min_dur", type=float, default=0.10)
    p.add_argument("--tol_cents", type=int, default=50)

    args = p.parse_args()

    pitch = read_pitch_csv(args.pitch_csv)
    segs = read_segments_csv(args.segments_csv)

    notes = build_notes(
        pitch=pitch,
        segs=segs,
        conf_threshold=args.conf_th,
        min_note_dur=args.min_dur,
        tol_cents=args.tol_cents,
    )

    payload = {
        "version": 1,
        "notes": notes,
        "segments": [{"start": s.start, "end": s.end, "is_vocal": s.is_vocal} for s in segs],
    }

    with open(args.out, "w", encoding="utf-8") as f:
        json.dump(payload, f, ensure_ascii=False, indent=2)

    print(f"Saved: {args.out}  (notes={len(notes)})")


if __name__ == "__main__":
    main()