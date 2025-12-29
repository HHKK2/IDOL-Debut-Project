using System;
using UnityEngine;

[Serializable]
public class ComebackSongData
{
    [Header("Meta")]
    public string songId;
    public string title;
    public string concept;   // ⭐ 추가: 컨셉

    [Header("Visual")]
    public Sprite albumCover;

    [Header("Audio")]
    public AudioClip audioClip;
}
