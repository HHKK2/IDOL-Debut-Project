import argparse
import csv
from dataclasses import dataclass
from typing import List, Optional, Tuple

import numpy as np
import librosa

@dataclass
class PitchFrame:
    t: float # seconds
    f0_hz: Optional[float] # none if unvoice/unknown
    confidence: float # 0~1

def hz_to_midi(f0_hz: float) -> float:
    #MIDI note number (A4=440Hz -> 69)
    return 69.0 + 12.0 * np.log2(f0_hz / 440.0)

def estimate_pitch_yin(
        y: np.ndarray,
        sr: int,
        fmin: float,
        fmax: float,
        frame_length: int,
        hop_length: int,
        trough_threshold: float,
) -> Tuple[np.ndarray, np.ndarray]:
    f0 = librosa.yin(
        y=y,
        fmin=fmin,
        fmax=fmax,
        frame_length=frame_length,
        hop_length=hop_length,
        trough_threshold=trough_threshold,
    )
    times = librosa.frames_to_time(np.arange(len(f0)), sr=sr, hop_length=hop_length)
    return times, f0

def pitch_track(
        wav_path: str,
        sr: int = 22050,
        fmin: float = 80.0,
        fmax: float = 1000.0,
        frame_length: int = 2048,
        hop_length: int = 256,
        trough_threshold: float = 0.1,
        energy_gate_db: float = -35.0,
) -> List[PitchFrame]:
    y, file_sr = librosa.load(wav_path, sr=sr, mono=True)
    #정규화
    if np.max(np.abs(y)) > 0:
        y = y/np.max(np.abs(y))

    times, f0 = estimate_pitch_yin(
        y=y,
        sr=sr,
        fmin=fmin,
        fmax=fmax,
        frame_length=frame_length,
        hop_length=hop_length,
        trough_threshold=trough_threshold,
    )

    #low energy frames treated as unvoiced
    rms = librosa.feature.rms(y=y, frame_length=frame_length, hop_length=hop_length)[0]
    rms_db = librosa.amplitude_to_db(rms, ref=np.max)

    frames: List[PitchFrame] = []
    for t, f, edb in zip(times, f0, rms_db):
        if edb < energy_gate_db or np.isnan(f) or f <= 0:
            frames.append(PitchFrame(t=t, f0_hz=None, confidence=0.0))
        else:
            #if louder -> more confident
            conf = float(np.clip((edb - energy_gate_db) / (0-energy_gate_db), 0.0, 1.0))
            frames.append(PitchFrame(t=t, f0_hz=float(f), confidence=conf))
    return frames

def write_csv(frames: List[PitchFrame], out_csv: str) -> None:
    with open(out_csv, "w", newline='', encoding="utf-8") as f:
        w = csv.writer(f)
        w.writerow(["time_sec", "f0_hz", "midi", "confidence"])
        for fr in frames:
            if fr.f0_hz is None:
                w.writerow([f"{fr.t:.6f}", "", "", f"{fr.confidence:.3f}"])
            else:
                midi = hz_to_midi(fr.f0_hz)
                w.writerow([f"{fr.t:.6f}", f"{fr.f0_hz:.3f}", f"{midi:.3f}"])


def main():
    p = argparse.ArgumentParser()
    p.add_argument("wav", help="Path to input wav file")
    p.add_argument("--out", default="pitch.csv", help="Path to output csv file")
    p.add_argument("--sr", type=int, default=22050, help="Sampling rate" )
    p.add_argument("--fmin", type=float, default=80.0, help="minimun frequency of pitch in Hz" )
    p.add_argument("--fmax", type=float, default=1000.0, help="maximum frequency of pitch in Hz" )
    p.add_argument("--frame", type=int, default=2048)
    p.add_argument("--hop", type=int, default=256)
    p.add_argument("--gate_db", type=float, default=-35.0, help="gate db")
    args = p.parse_args()

    frames = pitch_track(
        wav_path=args.wav,
        sr=args.sr,
        fmin=args.fmin,
        fmax=args.fmax,
        frame_length=args.frame,
        hop_length=args.hop,
        energy_gate_db=args.gate_db,
    )
    write_csv(frames, args.out)
    print(f"Saved: {args.out}  (frames={len(frames)})")

if __name__ == "__main__":
    main()