using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KaraokeSongData
{
    public List<KaraokeLine> lines;

    public KaraokeLine GetLineAtTime(float t)
    {
        if (lines == null || lines.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            if (t >= line.start && t <= line.end)
            {
                return line;
            }
        }

        return null;
    }
}

[Serializable]
public class KaraokeLine
{
    public int line_index;
    public string singer;
    public string text;
    public float start;
    public float end;
    public List<KaraokeSyllable> syllables;

    public int GetSyllableIndexAtTime(float t)
    {
        if (syllables == null || syllables.Count == 0)
        {
            return -1;
        }

        for (int i = 0; i < syllables.Count; i++)
        {
            var s = syllables[i];
            if (t >= s.start && t < s.end)
            {
                return i;
            }
        }

        return -1;
    }
}

[Serializable]
public class KaraokeSyllable
{
    public string ch;
    public float start;
    public float end;
}
