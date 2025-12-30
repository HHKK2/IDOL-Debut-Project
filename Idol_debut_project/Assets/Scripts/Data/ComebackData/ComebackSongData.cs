using System;
using System.Net.Mime;
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

    public TextAsset karaokeJsonAsset;
    public TextAsset scoreChartJsonAsset;

    // ───── 외부 접근용 ─────
    public bool UsedInComeback => usedInComeback;

    //너는 이미 컴백을 했단다. 이제부터 연습하기에서 재생이 되렴.
    public void MarkUsedInComeback()
    {
        usedInComeback = true;
    }

    //초기에 시작할 때 컴백했음을 전부 초기화해줍니다.
    public void ResetUsedInComeback()
    {
        usedInComeback = false;
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
