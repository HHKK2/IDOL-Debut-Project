using System;
using UnityEngine;

public class VocalJudge : MonoBehaviour
{
    public AudioInput audioInput;
    public PitchDetector pitchDetector;
    public VocalActivityDetector vocalDetector;
    public ScoreChart ScoreChart;

    [Header("Timing")] public float songTimeSec = 0.0f;
    public bool useAudioSourceTime = true;
    public AudioSource songAudioSource;

    [Header("Debug")] public bool logDebug = false;
    
    public int Score { get; private set; }
    public string LastJudgement { get; private set; }

    void Start()
    {
        Score = 0;
        LastJudgement = "";
    }

    void Update()
    {
        if (audioInput == null || pitchDetector == null || vocalDetector == null || ScoreChart == null)
        {
            return;
        }

        if (useAudioSourceTime && songAudioSource != null)
        {
            songTimeSec = songAudioSource.time;
        }
        else
        {
            songTimeSec += Time.deltaTime;
        }

        float[] frame;
        
        if(!audioInput.TryGetFrame(out frame)) return;
        
        vocalDetector.Analyze(frame);

        if (vocalDetector.IsVocalActive)
        {
            pitchDetector.Analyze(frame);
        }
        else
        {
            pitchDetector.Analyze(null);
        }

        ScoreChart.Note note = ScoreChart.GetActiveNote(songTimeSec);
        Judge(note);
    }

    void Judge(ScoreChart.Note note)
    {
        if (note == null)
        {
            if (vocalDetector.IsVocalActive)
            {
                LastJudgement = "ShouldBeSilent";
                Score -= 1;
            }
            else
            {
                LastJudgement = "SilentOK";
            }
        }
        else
        {
            if (!vocalDetector.IsVocalActive)
            {
                LastJudgement = "Miss(Silent)";
                Score -= 2;
            }
            else
            {
                int expected = note.midi;
                int actual = Mathf.RoundToInt(pitchDetector.LastMidi);

                if (pitchDetector.Confidence < 0.15f)
                {
                    LastJudgement = "Uncertain";
                    Score -= 1;
                }
                else if (actual == expected)
                {
                    LastJudgement = "Good";
                    Score += 2;
                }
                else
                {
                    {
                        LastJudgement = "Bad";
                        Score -= 1;
                    }
                }
            }

            if (logDebug)
            {
                Debug.Log(
                    "t=" + songTimeSec.ToString("F2") + 
                    " vocal=" + vocalDetector.IsVocalActive + 
                    " f0=" + pitchDetector.LastF0Hz.ToString("F1") + 
                    " midi=" + pitchDetector.LastMidi.ToString("F2") +
                    " conf=" + pitchDetector.Confidence.ToString("F2") +
                    " judgement=" + LastJudgement + " score=" + Score
                    );
            }
        }
    }
}
