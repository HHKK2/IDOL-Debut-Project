using System;
using UnityEngine;

[Serializable]
public class ComebackSongData
{
    [Header("Meta")]
    public string songId;
    public string title;
    public string concept;

    [Header("Visual")]
    public Sprite albumCover;

    [Header("Audio")]
    public SongAudioVariant audio;

    [Header("State")]
    [SerializeField] private bool usedInComeback;

    // ───── 외부 접근용 ─────
    public bool UsedInComeback => usedInComeback;

    public void MarkUsedInComeback()
    {
        usedInComeback = true;
    }

    public AudioClip GetPracticeClip()
    {
        return audio.original;
    }

    public AudioClip GetComebackClip()
    {
        return audio.inst;
    }
}
