using System;
using UnityEngine;

public class VocalJudge : MonoBehaviour
{
    public AudioInput audioInput;
    public PitchDetector pitchDetector;
    public VocalActivityDetector vocalDetector;
    public ScoreChart scoreChart;   // 변수명 소문자 추천

    [Header("Timing")]
    public bool useAudioSourceTime = true;
    public AudioSource songAudioSource;

    [Header("Judge Rate")]
    public float judgeIntervalSec = 0.25f; // 0.25초마다 판정
    private float judgeTimer = 0.0f;

    [Header("Result")] 
    public bool autoFinishWhenSongEnds = true;
    public float endGraceSec = 0.25f;
    private bool finished = false;

    [Header("Debug")]
    public bool logDebug = false;

    public int Score { get; private set; }
    public int PerfectCount { get; private set; }
    public int GoodCount { get; private set; }
    public int BadCount { get; private set; }
    public int MissCount { get; private set; }
    public int SilentPenaltyCount { get; private set; }
    
    public int TotalCount { get; private set; }

    public float PitchAccuracy { get; private set; }
    
    public string LastJudgement { get; private set; }

    public event Action<VocalResult> OnFinished;
    private float songTimeSec = 0.0f;
    void Start()
    {
        ResetResult();
    }

    void Update()
    {
        if(finished) return;
        if (audioInput == null || pitchDetector == null || vocalDetector == null || scoreChart == null)
            return;

        // 시간 업데이트
        if (useAudioSourceTime && songAudioSource != null)
            songTimeSec = songAudioSource.time;
        else
            songTimeSec += Time.deltaTime;

        if (autoFinishWhenSongEnds && songAudioSource != null)
        {
            bool ended = (!songAudioSource.isPlaying) && (songAudioSource.time > 0.05f);
            if (!ended && songAudioSource.clip != null)
            {
                float len = songAudioSource.clip.length;
                if (songAudioSource.time >= len - endGraceSec) ended = true;
            }

            if (ended)
            {
                Finish();
                return;
            }
        }

        // 오디오 프레임 받기
        float[] frame;
        if (!audioInput.TryGetFrame(out frame))
            return;

        // 보컬 활동 감지
        vocalDetector.Analyze(frame);

        // 보컬일 때만 pitch 분석 (null 넣지 말자)
        if (vocalDetector.IsVocalActive)
            pitchDetector.Analyze(frame);
        else
            pitchDetector.Reset(); // PitchDetector에 Reset() 만들어두는 걸 추천

        // 판정은 매 프레임 말고 N초마다
        judgeTimer += Time.deltaTime;
        if (judgeTimer < judgeIntervalSec) return;
        judgeTimer = 0.0f;

        // 현재 노트 찾기
        ScoreChart.Note note = scoreChart.GetNoteAtTime(songTimeSec);

        // 판정
        Judge(note);

        // note가 null이든 아니든 디버그 출력
        if (logDebug)
        {
            string noteStr = (note == null) ? "none" : ("midi=" + note.midi);
            Debug.Log(
                "t=" + songTimeSec.ToString("F2") +
                " vocal=" + vocalDetector.IsVocalActive +
                " f0=" + pitchDetector.LastF0Hz.ToString("F1") +
                " midi=" + pitchDetector.LastMidi.ToString("F2") +
                " conf=" + pitchDetector.Confidence.ToString("F2") +
                " note=" + noteStr +
                " judgement=" + LastJudgement +
                " score=" + Score
            );
        }
    }

    void Judge(ScoreChart.Note note)
    {
        TotalCount++;
        // note 없는데 소리 내면 페널티
        if (note == null)
        {
            if (vocalDetector.IsVocalActive)
            {
                LastJudgement = "ShouldBeSilent";
                Score -= 6;
                SilentPenaltyCount++;
            }
            else
            {
                LastJudgement = "SilentOK";
            }
            return;
        }

        // note 있는데 조용하면 Miss
        if (!vocalDetector.IsVocalActive)
        {
            LastJudgement = "Miss(Silent)";
            Score -= 4;
            MissCount++;
            return;
        }

        // confidence 낮으면 불확실
        if (pitchDetector.Confidence < 0.0f)
        {
            LastJudgement = "Uncertain";
            Score -= 1;
            BadCount++;
            return;
        }

        // tol_cents 기반 판정 (권장)
        float expectedMidi = note.midi;
        float actualMidi = pitchDetector.LastMidi;

        float cents = Mathf.Abs(actualMidi - expectedMidi) * 1.0f;
        int tol = note.tol_cents > 0 ? note.tol_cents : 80;

        if (cents <= tol*0.5f)
        {
            LastJudgement = "Perfect";
            Score += 20;
            PerfectCount++;
        }
        else if (cents <= tol)
        {
            LastJudgement = "Good";
            Score += 10;
            GoodCount++;
        }
        else
        {
            LastJudgement = "Bad";
            Score -= 10;
            BadCount++;
        }
    }

    public void Finish()
    {
        if(finished) return;
        finished = true;

        VocalResult result = new VocalResult();
        result.score = Score;
        result.perfect = PerfectCount;
        result.good = GoodCount;
        result.bad = BadCount;
        result.miss = MissCount;
        result.silentPenalty = SilentPenaltyCount;

        if (OnFinished != null) OnFinished(result);
        
        Debug.Log("[Vocal Judge] FINISHED! score=" + result.score +
                  " perfect="+result.perfect + 
                  " good="+result.good +
                  " bad=" + result.bad +
                  " miss=" + result.miss +
                  " silentPenalty=" + result.silentPenalty);
    }

    public void ResetResult()
    {
        finished = false;
        songTimeSec = 0.0f;
        judgeTimer = 0.0f;

        Score = 0;
        PerfectCount = 0;
        GoodCount = 0;
        BadCount = 0;
        MissCount = 0;
        SilentPenaltyCount = 0;
        TotalCount = 0;
        LastJudgement = "";

    }
}

[Serializable]
public class VocalResult
{
    public int score;
    public int perfect;
    public int good;
    public int bad;
    public int miss;
    public int silentPenalty;
    public int totalCount;
}