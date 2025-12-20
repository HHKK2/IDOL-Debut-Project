# idol_vocal_analyzer/export_song.py
import argparse
import subprocess
import sys
from pathlib import Path


def run(cmd: list) -> None:
    subprocess.run(cmd, check=True)


def ensure_wav(audio_path: Path, work_dir: Path) -> Path:
    """
    Return a .wav path to use for analysis.
    - If input is wav: return it.
    - If input is mp3 (or others): convert to wav into work_dir via ffmpeg.
    """
    suffix = audio_path.suffix.lower()
    if suffix == ".wav":
        return audio_path

    # Convert to wav
    wav_path = work_dir / (audio_path.stem + ".wav")
    run([
        "ffmpeg", "-y",
        "-i", str(audio_path),
        "-ac", "1",        # mono
        "-ar", "22050",    # match your default sr
        str(wav_path)
    ])
    return wav_path


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--audio", required=True, help="path to input audio (wav/mp3)")
    p.add_argument("--segments_csv", required=True, help="human-made segments.csv")
    p.add_argument("--out_dir", default="out", help="base output directory")
    p.add_argument("--song_id", default=None, help="optional song id (default: audio filename)")
    args = p.parse_args()

    audio_path = Path(args.audio)
    seg_path = Path(args.segments_csv)

    if not audio_path.exists():
        raise FileNotFoundError("audio not found: " + str(audio_path))
    if not seg_path.exists():
        raise FileNotFoundError("segments_csv not found: " + str(seg_path))

    song_id = args.song_id if args.song_id else audio_path.stem

    base_out = Path(args.out_dir)
    song_out = base_out / song_id
    song_out.mkdir(parents=True, exist_ok=True)

    # Always analyze a wav (convert if needed)
    wav_path = ensure_wav(audio_path, song_out)

    pitch_csv = song_out / "pitch.csv"
    seg_norm_csv = song_out / "segments.norm.csv"
    chart_json = song_out / "score_chart.json"
    meta_json = song_out / "song_meta.json"

    py = sys.executable  # uses current venv python

    # 1) pitch.csv
    run([py, "-m", "idol_vocal_analyzer.analyze_pitch", str(wav_path), "--out", str(pitch_csv)])

    # 2) segments.norm.csv
    run([py, "-m", "idol_vocal_analyzer.vocal_segments", str(seg_path), "--out", str(seg_norm_csv)])

    # 3) score_chart.json
    run([
        py, "-m", "idol_vocal_analyzer.build_chart",
        "--pitch_csv", str(pitch_csv),
        "--segments_csv", str(seg_norm_csv),
        "--out", str(chart_json),
    ])

    # 4) song_meta.json
    run([
        py, "-m", "idol_vocal_analyzer.song_meta",
        "--pitch_csv", str(pitch_csv),
        "--out", str(meta_json),
    ])

    print("DONE")
    print("song_id:", song_id)
    print("- " + str(pitch_csv))
    print("- " + str(seg_norm_csv))
    print("- " + str(chart_json))
    print("- " + str(meta_json))


if __name__ == "__main__":
    main()