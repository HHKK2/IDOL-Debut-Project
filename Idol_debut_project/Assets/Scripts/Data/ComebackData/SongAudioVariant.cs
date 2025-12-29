using System;
using UnityEngine;

[Serializable]
public class SongAudioVariant
{
    [Header("Audio Versions")]
    public AudioClip original;   // Practice용 (보컬 그대로)
    public AudioClip inst;       // Comeback용 (보컬 작아진 버전)
}
