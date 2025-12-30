# idol_vocal_analyzer/batch_export_all.py
from __future__ import annotations

import subprocess
import sys
from pathlib import Path


def tool_root() -> Path:
    # 이 파일이 있는 폴더가 곧 루트 (여기 안에 data/, analyze_pitch.py 등이 있음)
    root = Path(__file__).resolve().parent
    if not (root / "data").exists():
        raise FileNotFoundError(f"[ROOT] data 폴더를 찾을 수 없음: {root}")
    return root


def run(cmd: list[str], cwd: Path) -> None:
    print("  $", " ".join(cmd))
    subprocess.run(cmd, cwd=str(cwd), check=True)


def ensure_wav(mp3_path: Path, out_dir: Path, sr: int = 22050) -> Path:
    """
    mp3 -> wav 변환(항상 mono, 22050)
    이미 wav가 있으면 그대로 사용
    """
    if mp3_path.suffix.lower() == ".wav":
        return mp3_path

    wav_path = out_dir / f"{mp3_path.stem}.wav"
    run(
        [
            "ffmpeg", "-y",
            "-i", str(mp3_path),
            "-ac", "1",
            "-ar", str(sr),
            str(wav_path),
        ],
        cwd=out_dir
    )
    return wav_path


def main() -> None:
    root = tool_root()
    py = sys.executable

    data_dir = root / "data"
    raw_dir = data_dir / "raw"
    seg_dir = data_dir / "segments"
    lyr_dir = data_dir / "lyrics_combined"
    out_dir = data_dir / "output"

    out_dir.mkdir(parents=True, exist_ok=True)

    # 스크립트 파일 경로들(= -m 안 쓰고 직접 실행)
    analyze_pitch_py = root / "analyze_pitch.py"
    vocal_segments_py = root / "vocal_segments.py"
    build_chart_py = root / "build_chart.py"
    song_meta_py = root / "song_meta.py"
    karaoke_lyrics_py = root / "karaoke_lyrics.py"

    for p in [analyze_pitch_py, vocal_segments_py, build_chart_py, song_meta_py, karaoke_lyrics_py]:
        if not p.exists():
            raise FileNotFoundError(f"필수 스크립트 없음: {p}")

    mp3s = sorted(raw_dir.glob("*.mp3"))
    if not mp3s:
        raise FileNotFoundError(f"raw mp3가 없음: {raw_dir}")

    ok = 0
    skipped = 0

    print("[ROOT]", root)
    print("[RAW ]", raw_dir)
    print("[SEGS]", seg_dir)
    print("[LYR ]", lyr_dir)
    print("[OUT ]", out_dir)

    for mp3 in mp3s:
        song_key = mp3.stem  # 예: F1_our_spring

        seg_csv = seg_dir / f"{song_key}_segments.csv"
        lyr_csv = lyr_dir / f"{song_key}_lyrics.csv"

        if not seg_csv.exists() or not lyr_csv.exists():
            print(f"\n[SKIP] {song_key} (segments={seg_csv.exists()} / lyrics={lyr_csv.exists()})")
            skipped += 1
            continue

        song_out = out_dir / song_key
        song_out.mkdir(parents=True, exist_ok=True)

        pitch_csv = song_out / "pitch.csv"
        seg_norm_csv = song_out / "segments.norm.csv"
        chart_json = song_out / "score_chart.json"
        meta_json = song_out / "song_meta.json"
        karaoke_json = song_out / "karaoke.json"

        print(f"\n=== {song_key} ===")

        # 0) mp3 -> wav
        wav_path = ensure_wav(mp3, song_out)

        # 1) pitch.csv
        print("1) analyze_pitch -> pitch.csv")
        run(
            [py, str(analyze_pitch_py), str(wav_path), "--out", str(pitch_csv)],
            cwd=root,
        )

        # 2) segments.norm.csv
        print("2) vocal_segments -> segments.norm.csv")
        run(
            [py, str(vocal_segments_py), str(seg_csv), "--out", str(seg_norm_csv)],
            cwd=root,
        )

        # 3) score_chart.json
        print("3) build_chart -> score_chart.json")
        run(
            [
                py, str(build_chart_py),
                "--pitch_csv", str(pitch_csv),
                "--segments_csv", str(seg_norm_csv),
                "--out", str(chart_json),
            ],
            cwd=root,
        )

        # 4) song_meta.json
        print("4) song_meta -> song_meta.json")
        run(
            [py, str(song_meta_py), "--pitch_csv", str(pitch_csv), "--out", str(meta_json)],
            cwd=root,
        )

        # 5) karaoke.json (합쳐진 lyrics + score_chart notes 이용)
        print("5) karaoke_lyrics -> karaoke.json")
        run(
            [
                py, str(karaoke_lyrics_py),
                "--lyrics_csv", str(lyr_csv),
                "--chart_json", str(chart_json),
                "--out", str(karaoke_json),
            ],
            cwd=root,
        )

        # sanity check
        for outp in [pitch_csv, seg_norm_csv, chart_json, meta_json, karaoke_json]:
            if not outp.exists():
                raise FileNotFoundError(f"출력 파일이 안 만들어짐: {outp}")

        ok += 1
        print(f"[OK] {song_key} -> {song_out}")

    print(f"\nDONE. ok={ok}, skipped={skipped}")
    print("OUTPUT DIR:", out_dir)


if __name__ == "__main__":
    main()