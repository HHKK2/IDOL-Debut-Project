using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Karaoke/SongLyrics")]
public class SongLyrics : ScriptableObject
{
    [Serializable]
    public class Line
    {
        public float time;
        [TextArea] public string text;
    }

    public List<Line> lines = new List<Line>();
}
