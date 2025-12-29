using System;
using UnityEngine;

[Serializable]
public class ComebackEntry
{
    [Header("Order")]
    public int order;   // 0-base

    [Header("Songs By Gender")]
    public ComebackSongData maleSong;
    public ComebackSongData femaleSong;

    public ComebackSongData GetSongByGender(Gender gender)
    {
        return gender == Gender.MALE ? maleSong : femaleSong;
    }
}
