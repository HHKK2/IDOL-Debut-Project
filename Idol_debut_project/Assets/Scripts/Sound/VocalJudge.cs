using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using Data;
using UnityEngine;
using System.Collections.Generic;

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
    public float judgeIntervalSec = 0.05f; // 0.05초마다 판정
    private float judgeTimer = 0.0f;

    [Header("Result")] 
    public bool autoFinishWhenSongEnds = true;
    public float endGraceSec = 0.25f;
    private bool finished = false;
    
    [Header("Final Score Tuning")]
    [Range(0f, 2f)]
    public float penaltyScaleForFinal = 0.2f; // 1.0이면 페널티 빡셈, 0.6이면 덜 빡셈

    [Header("Score Normalization")] 
    [Range(0f, 1f)] public float goodWeight = 1.0f;

    [Range(0f, 1f)] public float badWeight = 0.20f;

    [Range(0.3f, 1.0f)] public float perfectRatio = 0.75f;
    

    public float missPenaltyWeight = 0.2f;
    public float silentPenaltyWeight = 0.3f;
    public int PitchScore100 { get; private set; }
    public int Penalty100 { get; private set; }
    public int FinalScore100 { get; private set; }
    public AudianceData.EAudianceFeeling Feeling { get; private set; }

    private int GetPitchTrials()
    {
        return PerfectCount + GoodCount + BadCount + MissCount;
    }

    private void RecalculateScores()
    {
        int pitchTrials = GetPitchTrials();
        if (pitchTrials <= 0)
        {
            PitchScore100 = 0;
            Penalty100 = 0;
            FinalScore100 = 0;
            Feeling = AudianceFeelingUtil.FromScore100(0);
            return;
        }

        float raw = (PerfectCount * 1.0f) + (GoodCount * goodWeight) + (BadCount * badWeight);
        float pitchAcc = (raw / pitchTrials) * 100.0f;
        PitchScore100 = Mathf.Clamp(Mathf.RoundToInt(pitchAcc), 0, 100);

        float penaltyPoints = (MissCount * missPenaltyWeight) + (SilentPenaltyCount * silentPenaltyWeight);
        float maxPenaltyPoints = pitchTrials * silentPenaltyWeight;
        float penaltyRate = (maxPenaltyPoints <= 0f) ? 0f : (penaltyPoints / maxPenaltyPoints) * 100f;
        Penalty100 = Mathf.Clamp(Mathf.RoundToInt(penaltyRate), 0, 100);

        float penaltyScale = penaltyScaleForFinal;
        int final = Mathf.RoundToInt(PitchScore100 - Penalty100 * penaltyScale);
        FinalScore100 = Mathf.Clamp(final, 0, 100);
        Feeling = AudianceFeelingUtil.FromScore100(FinalScore100);
    }

    [Header("Debug")]
    public bool logDebug = false;
    
    [Header("Judgement Tuning")]
    [Range(0.5f, 3.0f)]
    public float judgementToleranceScale = 1.4f; // 1.0=원래, 1.4=널널, 1.8=더 널널

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
        Debug.Log($"[VocalJudge] START id={GetInstanceID()} name={gameObject.name} active={gameObject.activeInHierarchy}");
        ResetResult();
        RecalcAudienceWindow();
    }

    void RecalcAudienceWindow()
    {
        maxSamplesPerWindow = Mathf.Max(1, Mathf.RoundToInt(audienceWindowSec / judgeIntervalSec));
        
        // PitchDetector의 sampleRate를 AudioInput과 동기화
        if (audioInput != null && pitchDetector != null)
        {
            pitchDetector.sampleRate = audioInput.sampleRate;
            Debug.Log($"[VocalJudge] PitchDetector sampleRate 동기화: {pitchDetector.sampleRate}");
        }
    }

    void Update()
    {
        if(finished) return;
        if (audioInput == null || pitchDetector == null || vocalDetector == null || scoreChart == null)
        {
            Debug.LogWarning("[VocalJudge] 컴포넌트가 null입니다! audioInput=" + (audioInput != null) + 
                            " pitchDetector=" + (pitchDetector != null) + 
                            " vocalDetector=" + (vocalDetector != null) + 
                            " scoreChart=" + (scoreChart != null));
            return;
        }

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
        {
            Debug.LogWarning("[VocalJudge] 마이크 입력 실패! IsReady=" + audioInput.IsReady);
            return;
        }

        // 보컬 활동 감지
        vocalDetector.Analyze(frame);

        // 보컬일 때만 pitch 분석 (null 넣지 말자)
        if (vocalDetector.IsVocalActive)
        {
            pitchDetector.Analyze(frame);
            Debug.Log($"[VocalJudge] 보컬 감지: IsVocalActive={vocalDetector.IsVocalActive}, RMS={vocalDetector.LastRms:F4}, " +
                     $"MIDI={pitchDetector.LastMidi:F2}, Hz={pitchDetector.LastF0Hz:F1}, Confidence={pitchDetector.Confidence:F2}");
        }
        else
        {
            pitchDetector.Reset();
        }

        // 판정은 매 프레임 말고 N초마다
        judgeTimer += Time.deltaTime;
        if (judgeTimer < judgeIntervalSec) return;
        judgeTimer = 0.0f;

        // 현재 노트 찾기
        ScoreChart.Note note = scoreChart.GetNoteAtTime(songTimeSec);
        
        if (note != null)
        {
            Debug.Log($"[VocalJudge] 현재 노트: midi={note.midi}, tol_cents={note.tol_cents}, time={songTimeSec:F2}");
        }

        // 판정 전 상태 저장
        int scoreBefore = Score;
        int perfectBefore = PerfectCount;
        int goodBefore = GoodCount;
        int badBefore = BadCount;
        int missBefore = MissCount;
        string judgementBefore = LastJudgement;

        // 판정
        Judge(note);

        // 판정 후 값 변화 확인
        if (Score != scoreBefore || PerfectCount != perfectBefore || 
            GoodCount != goodBefore || BadCount != badBefore || MissCount != missBefore)
        {
            Debug.Log($"[VocalJudge] ⭐ 판정 결과: {LastJudgement} | " +
                     $"Score: {scoreBefore} → {Score} ({Score - scoreBefore:+0;-0}) | " +
                     $"Perfect: {PerfectCount}, Good: {GoodCount}, Bad: {BadCount}, Miss: {MissCount}");
        }

        audienceTimer += Time.deltaTime;
        if (audienceTimer >= audienceUpdateIntervalSec)
        {
            audienceTimer = 0f;
            AudianceData.EAudianceFeeling feelingBefore = Feeling;
            var feeling = CalculateWindowFeeling();
            Feeling = feeling;
            
            if (true)
            {
                Debug.Log($"[VocalJudge] 🎭 Feeling 업데이트: {feelingBefore} → {feeling} | windowSamples.Count={windowSamples.Count}");
            }
            
            OnAudienceFeelingUpdated?.Invoke(feeling);
        }

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
        
       
        
        // --- audience window timer는 "실시간"으로 돌린다 ---
        // audienceTimer += Time.deltaTime;
        // if (audienceTimer >= audienceUpdateIntervalSec)
        // {
        //     audienceTimer = 0f;
        //
        //     var feeling = CalculateWindowFeeling();
        //
        //     Debug.Log(
        //         $"[AudienceWindow] t={songTimeSec:F2}s feeling={feeling} " +
        //         $"samples={windowSamples.Count} " +
        //         $"P/G/B/M/S={CountKind(JudgeKind.Perfect)}/{CountKind(JudgeKind.Good)}/{CountKind(JudgeKind.Bad)}/{CountKind(JudgeKind.Miss)}/{CountKind(JudgeKind.SilentPenalty)}"
        //     );
        //
        //     OnAudienceFeelingUpdated?.Invoke(feeling);
        // }
    }
    int CountKind(JudgeKind kind)
    {
        int c = 0;
        foreach (var s in windowSamples) if (s == kind) c++;
        return c;
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
                AddWindowSample(JudgeKind.SilentPenalty);
                Debug.Log($"[Judge] ⚠️ 페널티: 노트 없는데 소리냄 | Score: {Score + 6} → {Score}");
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
            AddWindowSample(JudgeKind.Miss);
            Debug.Log($"[Judge] ❌ Miss: 노트 있는데 조용함 | Score: {Score + 4} → {Score}");
            return;
        }

        // confidence 낮으면 불확실
        if (pitchDetector.Confidence < 0.0f)
        {
            LastJudgement = "Uncertain";
            Score -= 1;
            BadCount++;
            Debug.Log($"[Judge] ⚠️ Uncertain: Confidence 낮음 ({pitchDetector.Confidence:F2}) | Score: {Score + 1} → {Score}");
            return;
        }

        // tol_cents 기반 판정 (권장)
        float expectedMidi = note.midi;
        float actualMidi = pitchDetector.LastMidi;

        float cents = Mathf.Abs(actualMidi - expectedMidi) * 100.0f;
        int basetol = note.tol_cents > 0 ? note.tol_cents : 120;

        float tolScale = judgementToleranceScale;
        float tol = basetol * tolScale;

        Debug.Log($"[Judge] 음높이 비교: 기대={expectedMidi:F2}, 실제={actualMidi:F2}, 차이={cents:F2}센트, 허용={tol}센트");

        if (cents <= tol*perfectRatio)
        {
            LastJudgement = "Perfect";
            Score += 20;
            PerfectCount++;
            AddWindowSample(JudgeKind.Perfect);
            Debug.Log($"[Judge] ✅ Perfect! | Score: {Score - 20} → {Score} | PerfectCount: {PerfectCount}");
        }
        else if (cents <= tol)
        {
            LastJudgement = "Good";
            Score += 10;
            GoodCount++;
            AddWindowSample(JudgeKind.Good);
            Debug.Log($"[Judge] ✓ Good | Score: {Score - 10} → {Score} | GoodCount: {GoodCount}");
        }
        else
        {
            LastJudgement = "Bad";
            Score -= 10;
            BadCount++;
            AddWindowSample(JudgeKind.Bad);
            Debug.Log($"[Judge] ✗ Bad | Score: {Score + 10} → {Score} | BadCount: {BadCount}");
        }
    }

    public void Finish()
    {
        if(finished) return;
        finished = true;
        
        RecalculateScores();

        VocalResult result = new VocalResult();
        result.score = Score;
        result.totalCount = TotalCount;
        
        result.perfect = PerfectCount;
        result.good = GoodCount;
        result.bad = BadCount;
        result.miss = MissCount;
        result.silentPenalty = SilentPenaltyCount;

        result.pitchScore100 = PitchScore100;
        result.penalty100 = Penalty100;
        result.finalScore100 = FinalScore100;
        result.feeling = Feeling.ToString();

        if (OnFinished != null) OnFinished(result);
        
        Debug.Log("[Vocal Judge] FINISHED! score=" + result.score +
                  " perfect="+result.perfect + 
                  " good="+result.good +
                  " bad=" + result.bad +
                  " miss=" + result.miss +
                  " silentPenalty=" + result.silentPenalty + 
                  " pitch=" + PitchScore100 + " penalty=" + Penalty100 + " final=" + FinalScore100 + " feeling=" + Feeling);
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

        TotalCount = 0;
        PitchScore100 = 0;
        Penalty100 = 0;
        FinalScore100 = 0;
    }

    public event Action<AudianceData.EAudianceFeeling> OnAudienceFeelingUpdated;
    [Header("Audience Window")] 
    public float audienceWindowSec = 5f;
    public float audienceUpdateIntervalSec = 5f;

    private int maxSamplesPerWindow;
    private float audienceTimer = 0f;
    
    private enum JudgeKind
    {
        Perfect,
        Good,
        Bad,
        Miss,
        SilentPenalty
    }

    private readonly Queue<JudgeKind> windowSamples = new Queue<JudgeKind>();

    void AddWindowSample(JudgeKind kind)
    {
        windowSamples.Enqueue(kind);
        while (windowSamples.Count > maxSamplesPerWindow)
        {
            windowSamples.Dequeue();
        }
        
    }

    AudianceData.EAudianceFeeling CalculateWindowFeeling()
    {
        int wPerfect = 0, wGood = 0, wBad = 0, wMiss = 0, wSilent = 0;
        foreach (var s in windowSamples)
        {
            switch (s)
            {
                case JudgeKind.Perfect: wPerfect++; break;
                case JudgeKind.Good: wGood++; break;
                case JudgeKind.Bad: wBad++; break;
                case JudgeKind.Miss: wMiss++; break;
                case JudgeKind.SilentPenalty: wSilent++; break;
            }
        }

        int trials = wPerfect + wGood + wBad + wMiss;
        if (trials <= 0)
        {
            Debug.Log("[CalculateWindowFeeling] 판정 데이터 없음 → Bad");
            return AudianceFeelingUtil.FromScore100(0);
        }

        float raw = (wPerfect * 1.0f) + (wGood * goodWeight) + (wBad * badWeight);
        float pitchAcc = (raw / trials) * 100.0f;

        float penaltyPoints = (wMiss * missPenaltyWeight) + (wSilent * silentPenaltyWeight);
        float maxPenaltyPoints = trials * silentPenaltyWeight;
        float penaltyRate = (maxPenaltyPoints <= 0f) ? 0f : (penaltyPoints / maxPenaltyPoints) * 100f;

        float final = pitchAcc - penaltyRate;
        int final100 = Mathf.Clamp(Mathf.RoundToInt(final), 0, 100);

        AudianceData.EAudianceFeeling feeling = AudianceFeelingUtil.FromScore100(final100);
        Debug.Log($"[CalculateWindowFeeling] 최근 5초 판정: " +
                 $"Perfect={wPerfect}, Good={wGood}, Bad={wBad}, Miss={wMiss}, Silent={wSilent} | " +
                 $"정확도={pitchAcc:F1}%, 페널티={penaltyRate:F1}% | " +
                 $"최종점수={final100} → Feeling={feeling}");

        return feeling;
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

    public int pitchScore100;
    public int penalty100;
    public int finalScore100;
    public string feeling;
}