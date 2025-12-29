using TMPro;
using UnityEngine;

public class KaraokeLyricsController
{
    [Header("참조")]
    public AudioSource songAudioSource;
    public SongLyrics lyrics;

    [Header("UI")] 
    public TextMeshProUGUI currentLineText;
    public TextMeshProUGUI nextLineText;

    private int index = 0;

    private void Start()
    {
        index = 0;
        UpdateUI();
    }

    private void Update()
    {
        if (lyrics == null || songAudioSource == null) return;
        if (lyrics.lines == null || lyrics.lines.Count == 0) return;

        float t = songAudioSource.time;

        while (index + 1 < lyrics.lines.Count && t >= lyrics.lines[index + 1].time)
        {
            index++;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (index < 0 || index >= lyrics.lines.Count)
        {
            if (currentLineText) currentLineText.text = "";
            if (nextLineText) nextLineText.text = "";
            return;
        }

        if (currentLineText)
        {
            currentLineText.text = lyrics.lines[index].text;
        }

        if (nextLineText)
        {
            if (index + 1 < lyrics.lines.Count)
            {
                nextLineText.text = lyrics.lines[index + 1].text;
            }
            else
            {
                nextLineText.text = "";
            }
        }
    }

    public void ResetLyrics()
    {
        index = 0;
        UpdateUI();
    }
    
}
