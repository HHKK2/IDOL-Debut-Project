# idol_vocal_analyzer/export_song.py
import argparse
import subprocess
from pathlib import Path


def run(cmd: list) -> None:
    subprocess.run(cmd, check=True)


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--wav", required=True, help="path to input wav")
    p.add_argument("--segments_csv", required=True, help="human-made segments.csv")
    p.add_argument("--out_dir", default="out", help="output directory")
    args = p.parse_args()

    out_dir = Path(args.out_dir)
    out_dir.mkdir(parents=True, exist_ok=True)

    pitch_csv = out_dir / "pitch.csv"
    seg_norm_csv = out_dir / "segments.norm.csv"
    chart_json = out_dir / "score_chart.json"
    meta_json = out_dir / "song_meta.json"

    # 1) pitch.csv
    run(["python", "-m", "idol_vocal_analyzer.analyze_pitch", args.wav, "--out", str(pitch_csv)])

    # 2) segments.norm.csv
    run(["python", "-m", "idol_vocal_analyzer.vocal_segments", args.segments_csv, "--out", str(seg_norm_csv)])

    # 3) score_chart.json
    run([
        "python", "-m", "idol_vocal_analyzer.build_chart",
        "--pitch_csv", str(pitch_csv),
        "--segments_csv", str(seg_norm_csv),
        "--out", str(chart_json),
    ])

    # 4) song_meta.json
    run([
        "python", "-m", "idol_vocal_analyzer.song_meta",
        "--pitch_csv", str(pitch_csv),
        "--out", str(meta_json),
    ])

    print("DONE")
    print(f"- {pitch_csv}")
    print(f"- {seg_norm_csv}")
    print(f"- {chart_json}")
    print(f"- {meta_json}")


if __name__ == "__main__":
    main()