using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class KaraokeLinePlayer : MonoBehaviour
{
    public TextMeshProUGUI lyricsText;
    public AudioSource songAudioSource;
    [SerializeField] private KaraokeSongData data;

    public TextAsset jsonAsset;

    private int currentLineIndex = 0;

    [Header("Singer Colors")]
    [SerializeField] private Color playerColor  = new Color32(0xAF, 0x0F, 0xFA, 0xFF); // AF0FFA
    [SerializeField] private Color aiColor      = new Color32(0x7B, 0x7F, 0x83, 0xFF); // 7B7F83
    [SerializeField] private Color chorusColor  = new Color32(0x00, 0x59, 0xFF, 0xFF); // 0059FF
    [SerializeField] private Color baseWhite    = Color.white;
    private void Awake()
    {
        if (lyricsText != null)
        {
            data = JsonUtility.FromJson<KaraokeSongData>(jsonAsset.text);
        }
    }

    private void Update()
    {
        if (data == null || songAudioSource == null || lyricsText == null)
        {
            return;
        }
        
        float t = songAudioSource.time;
        var line = data.GetLineAtTime(t);
        if(line == null) return;

        switch (line.singer)
        {
            case "P":
            {
                var syll = GetSyllableAtTime(line, t);
                int highlightIndex = syll != null ? syll.index : -1;
                lyricsText.text = BuildColoredText(
                    line.text,
                    highlightIndex,
                    baseWhite,
                    playerColor
                );
                break;
            }
            case "M":
                lyricsText.text = WrapWholeLineWithColor(line.text, aiColor);
                break;
            case "ALL":
                lyricsText.text = WrapWholeLineWithColor(line.text, chorusColor);
                break;
            default:
                lyricsText.text = line.text;
                break;
        }
    }

    KaraokeLine GetLineAtTime(float t)
    {
        if (data == null || data.lines == null) return null;
        for (int i = 0; i < data.lines.Count; i++)
        {
            var line = data.lines[i];
            if (t >= line.start && t <= line.end) return line;
        }

        return null;
    }

    KaraokeSyllableIndexed GetSyllableAtTime(KaraokeLine line, float t)
    {
        if (line.syllables == null || line.syllables.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < line.syllables.Count; i++)
        {
            var s = line.syllables[i];
            if (t >= s.start && t < s.end)
            {
                return new KaraokeSyllableIndexed { syllable = s, index = i };
            }
        }

        return null;
    }

    string WrapWholeLineWithColor(string text, Color color)
    {
        string hex = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{hex}>{text}</color>";
    }

    string BuildColoredText(string text, int highlightIndex, Color normalColor, Color highlightColor)
    {
        string raw = text.Replace(" ", "");
        var sb = new StringBuilder();
        string normalHex = ColorUtility.ToHtmlStringRGB(normalColor);
        string highlightHex = ColorUtility.ToHtmlStringRGB(highlightColor);

        for (int i = 0; i < raw.Length; i++)
        {
            string ch = raw[i].ToString();
            if (i == highlightIndex)
            {
                sb.Append("<color=#").Append(highlightHex).Append(">").Append(ch).Append("</color>");
            }
            else
            {
                sb.Append("<color=#").Append(normalHex).Append(">").Append(ch).Append("</color>");
            }
        }

        return sb.ToString();
    }

    class KaraokeSyllableIndexed
    {
        public KaraokeSyllable syllable;
        public int index;
    }

    String BuildColoredText(string text, int highlightIndex)
    {
        var chars = text.Replace(" ", "");
        var sb = new StringBuilder();
        for (int i = 0; i < chars.Length; i++)
        {
            string ch = chars[i].ToString();
            if (i == highlightIndex)
            {
                sb.Append("<color=#7B7F83>").Append(ch).Append("</color>");
            }
            else
            {
                sb.Append(ch);
            }
            
        }

        return sb.ToString();
    }
}
