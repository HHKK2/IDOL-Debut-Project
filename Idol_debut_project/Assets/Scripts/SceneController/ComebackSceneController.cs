using System;
using UnityEngine;
using Data;

public class ComebackSceneController : MonoBehaviour
{
    public static event Action OnFinished;

    private enum Phase
    {
        Intro, //실장님 컨셉 설명
        Prepare, //메인메뉴 화면 + 10분 타이머
        Practice, //연습 화면 _ 타이머는 이어짐. 
        Stage // 무대
    }

    private Phase phase;

    private const float PRACTICE_DURATION = 600f;
    private float timer;
    private bool isClicked = false;

    //이번 컴백 송 데이터
    private ComebackSongData currentSong;

    //HUD
    private CombackNoticeHUD noticeHUD;
    private MainMenuHUD mainMenuHUD;
    private TimerHUD timerHUD;
    private PracticeHUD practiceHUD;
    private StageHUD stageHUD;
    private StageResultHUD resultHUD;


    //무대 관련
    private AudioSource audioSource;
    // 읽기 전용 timestamp - UpdateStage()에서만 Time.deltaTime으로 증가시킴 (외부에서 직접 수정 불가)
    private float songTimestamp;
    private bool isStagePlaying = false;
    private bool isWaitingForCountdown = true; // 카운트다운 대기 중
    private VocalJudge vocalJudge;
    private VocalResult vocalResult;
    private Coroutine countdownCoroutine;

    //연습 관련
    private bool isPracticePlaying = false;
    private float practiceSongTimestamp;

    private void Start()
    {
        // 씬 시작 시 AudioSource 정리 (이전 씬에서 남아있을 수 있음)
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            UnityEngine.Object.Destroy(audioSource);
            audioSource = null;
        }

        // PersistentScene을 포함한 모든 씬의 AudioSource 정지
        // (PersistentScene은 DontDestroyOnLoad라서 FindObjectsByType으로 찾을 수 있음)
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (AudioSource source in allAudioSources)
        {
            // 자기 자신의 AudioSource는 제외 (아직 생성 안 됨)
            if (source != audioSource && source.isPlaying)
            {
                source.Stop();
                Debug.Log($"[ComebackScene] AudioSource 정지: {source.name}, clip: {(source.clip != null ? source.clip.name : "null")}");
            }
        }

        currentSong = GameManager.Instance.CurrentComebackSong;
        if (currentSong == null)
        {
            Debug.LogError("[ComebackScene] CurrentComebackSong is null. ComeBack.Enter에서 SetCurrentComebackSong 했는지 확인");
            return;
        }
        EnterIntro();
    }

    //Intro
    private void EnterIntro()
    {
        phase = Phase.Intro;
        isClicked = false;
        Debug.Log("[컴백씬] 인트로");

        noticeHUD = UIManager.Instance.ShowHUDUI<CombackNoticeHUD>();
        noticeHUD.Init(
            $"Sprites/AlbumCovers/{currentSong.albumCover.name}",
            currentSong.concept,
            currentSong.title);

        noticeHUD.OnCombackPrepareStart += OnPrepareSignal;
    }

    private void OnPrepareSignal()
    {
        if (isClicked)
            return;

        isClicked = true;

        if (noticeHUD != null)
        {
            noticeHUD.OnCombackPrepareStart -= OnPrepareSignal;

            if (noticeHUD.gameObject != null)
            {
                UIManager.Instance.HUDList.Remove(noticeHUD);
                UnityEngine.Object.Destroy(noticeHUD.gameObject);
            }

            noticeHUD = null;
        }

        EnterPrepare();
    }

    //Prepare 컴백 준비 : 이제부터 타이머를 띄움 + 여기서 메인메뉴 화면을 다시 보여줌. 
    private void EnterPrepare()
    {
        if (phase == Phase.Prepare)
            return;

        phase = Phase.Prepare;
        Debug.Log("[컴백씬] 컴백준비단계 돌입");

        timer = 0f;

        // 타이머 HUD
        timerHUD = UIManager.Instance.ShowHUDUI<TimerHUD>();

        // 메인 메뉴 HUD
        mainMenuHUD = UIManager.Instance.ShowHUDUI<MainMenuHUD>();

        // 컴백 준비 상태 UI 세팅
        mainMenuHUD.CompackPrepareStarted();

        // 버튼 이벤트 연결
        mainMenuHUD.ClickedPracticeButton += EnterPractice;
        mainMenuHUD.ClickedComebackButton += EnterStage;
    }

    private void Update()
    {
        if (phase == Phase.Prepare || phase == Phase.Practice)
        {
            timer += Time.deltaTime;

            if (timerHUD != null)
                timerHUD.Init(FormatTime());

            if (timer >= PRACTICE_DURATION)
            {
                EnterStage();
            }
        }

        if (phase == Phase.Practice && isPracticePlaying)
        {
            UpdatePractice();
        }
        else if (phase == Phase.Stage && isStagePlaying)
        {
            UpdateStage();
        }
    }

    //practice 컴백 연습. 
    private void EnterPractice()
    {
        if (phase != Phase.Prepare)
            return;

        phase = Phase.Practice;
        Debug.Log("[컴백씬] 연습하기");

        practiceHUD = UIManager.Instance.ShowHUDUI<PracticeHUD>();
        practiceHUD.Init(
            currentSong.title,
            $"Sprites/AlbumCovers/{currentSong.albumCover.name}");

        //practiceHUD.SetSelectingMode();

        practiceHUD.onClickedPracticeButton += StartPractice;
        practiceHUD.onClickedExitButton += ExitPractice;
    }

    private void StartPractice()
    {
        Debug.Log("[ComebackScene] Practice Started");

        practiceHUD.SetPlayingMode();

        AudioClip practiceClip = currentSong?.GetPracticeClip();
        if (currentSong == null || practiceClip == null)
        {
            Debug.LogError("[ComebackScene] currentSong 또는 audioClip이 null입니다.");
            return;
        }

        // AudioSource 초기화 및 정리
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
        else
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = practiceClip;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // timestamp 초기화
        practiceSongTimestamp = 0f;
        isPracticePlaying = true;

        // 음악 재생 시작
        audioSource.Play();
    }

    private void ExitPractice()
    {
        Debug.Log("[ComebackScene] Practice Exit");

        // 연습 중이면 정지
        if (isPracticePlaying)
        {
            isPracticePlaying = false;
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        // PracticeHUD 닫기
        if (practiceHUD != null)
        {
            practiceHUD.onClickedPracticeButton -= StartPractice;
            practiceHUD.onClickedExitButton -= ExitPractice;
            CloseHUDIfExists(practiceHUD);
            practiceHUD = null;
        }

        // Prepare 단계로 돌아가기
        phase = Phase.Prepare;
    }

    //stage 무대
    private void EnterStage()
    {
        if (phase == Phase.Stage)
            return;

        phase = Phase.Stage;
        Debug.Log("[컴백씬] 무대");

        // 이벤트 구독 해제
        if (mainMenuHUD != null)
        {
            mainMenuHUD.ClickedPracticeButton -= EnterPractice;
            mainMenuHUD.ClickedComebackButton -= EnterStage;
        }

        if (practiceHUD != null)
        {
            practiceHUD.onClickedPracticeButton -= StartPractice;
            practiceHUD.onClickedExitButton -= ExitPractice;
        }

        // 연습 중이면 정지
        if (isPracticePlaying)
        {
            isPracticePlaying = false;
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        // 필요한 HUD만 선택적으로 닫기
        CloseHUDIfExists(mainMenuHUD);
        CloseHUDIfExists(practiceHUD);
        CloseHUDIfExists(timerHUD);

        mainMenuHUD = null;
        practiceHUD = null;
        timerHUD = null;

        stageHUD = UIManager.Instance.ShowHUDUI<StageHUD>();

        StartStagePerformance();
    }

    private void StartStagePerformance()
    {
        AudioClip comebackClip = currentSong?.GetComebackClip();
        if (currentSong == null || comebackClip == null)
        {
            Debug.LogError("[ComebackScene] currentSong 또는 audioClip이 null입니다.");
            FinishStage();
            return;
        }

        // 기존 코루틴 정지
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        // AudioSource 초기화 및 정리
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
        else
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = comebackClip;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // timestamp 초기화 (읽기 전용 - Time.deltaTime으로만 증가)
        songTimestamp = 0f;
        isStagePlaying = true;
        isWaitingForCountdown = true;

        // 카운트다운이 끝날 때까지 기다린 후 음악 재생 (StageFadeInSystemUI의 delayTime은 3초)
        countdownCoroutine = StartCoroutine(WaitForCountdownAndStartMusic());

        // VocalJudge 관련 초기화
        vocalJudge = FindFirstObjectByType<VocalJudge>();
        if (vocalJudge != null)
        {
            // audioSource 연결
            vocalJudge.songAudioSource = audioSource;
            vocalJudge.useAudioSourceTime = true;

            // OnFinished 이벤트 구독
            vocalJudge.OnFinished += OnVocalJudgeFinished;
        }
        else
        {
            Debug.LogWarning("[ComebackScene] VocalJudge를 찾을 수 없습니다. 결과 없이 진행합니다.");
        }
    }

    private void UpdateStage()
    {
        // 카운트다운 중이면 timestamp 업데이트 안 함
        if (isWaitingForCountdown)
            return;

        // timestamp 업데이트 (읽기 전용 - Time.deltaTime으로만 증가, 외부에서 직접 수정 불가)
        songTimestamp += Time.deltaTime;

        // 음악 종료 체크
        if (audioSource != null && audioSource.clip != null)
        {
            // AudioSource가 재생 중이 아니거나 timestamp가 음악 길이를 넘었으면 종료
            if (!audioSource.isPlaying || songTimestamp >= audioSource.clip.length)
            {
                OnStageFinished();
                return;
            }
        }
        else if (currentSong != null)
        {
            AudioClip comebackClip = currentSong.GetComebackClip();
            if (comebackClip != null)
            {
                // AudioSource가 없어도 timestamp로 체크
                if (songTimestamp >= comebackClip.length)
                {
                    OnStageFinished();
                    return;
                }
            }
        }

        // StageHUD 업데이트
        if (stageHUD != null)
        {
            // 타이머 업데이트
            AudioClip comebackClip = currentSong?.GetComebackClip();
            float songLength = comebackClip != null ? comebackClip.length : 0f;
            float sliderValue = songLength > 0 ? Mathf.Clamp01(songTimestamp / songLength) : 0f;
            string timerText = FormatSongTime(songTimestamp, songLength);
            stageHUD.InitSongTimerValue(sliderValue, timerText);

            // TODO: 가사 업데이트
            // 현재 timestamp에 맞는 가사를 찾아서 표시해야 합니다.
            // 예시: stageHUD.InitLyricsText(GetLyricsAtTime(songTimestamp));
            // 일단 제목으로 표시
            stageHUD.InitLyricsText(currentSong.title);

            // 관객 반응 업데이트 (VocalJudge 결과 기반)
            if (vocalJudge != null)
            {
                stageHUD.InitAudianceImage(vocalJudge.Feeling);
            }
        }
    }

    private void OnStageFinished()
    {
        isStagePlaying = false;
        isWaitingForCountdown = false;

        // 노래 정지
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // VocalJudge가 있으면 결과를 기다림 (OnVocalJudgeFinished에서 처리)
        // VocalJudge가 없으면 바로 결과 화면 표시
        if (vocalJudge == null)
        {
            ShowStageResult(null);
        }
        // VocalJudge가 있으면 OnVocalJudgeFinished에서 처리됨
    }

    private void OnVocalJudgeFinished(VocalResult result)
    {
        vocalResult = result;
        ShowStageResult(result);
    }

    private void ShowStageResult(VocalResult result)
    {
        // StageHUD 닫기
        if (stageHUD != null)
        {
            CloseHUDIfExists(stageHUD);
            stageHUD = null;
        }

        // StageResultHUD 표시
        resultHUD = UIManager.Instance.ShowHUDUI<StageResultHUD>();

        // 클릭 이벤트 구독 (클릭하면 메인 화면으로 이동)
        resultHUD.OnClickGoHomeButton += FinishStage;

        // 앨범 번호 (comebackCount는 0-base이므로 +1)
        int albumNth = TimeCycleManager.Instance.comebackCount + 1;

        if (result != null)
        {
            // VocalJudge 결과가 있는 경우
            string scoreText = result.finalScore100.ToString();
            string rankImagePath = GetRankImagePath(result.finalScore100);

            // feeling 문자열을 EAudianceFeeling enum으로 변환
            AudianceData.EAudianceFeeling feeling = AudianceFeelingUtil.FromScore100(result.finalScore100);

            // 평판이 양수인지 여부 (점수가 50 이상이면 양수)
            bool isReputationPositive = result.finalScore100 >= 50;

            resultHUD.Init(rankImagePath, scoreText, albumNth.ToString(), currentSong.title, feeling, isReputationPositive);
        }
        else
        {
            // VocalJudge 결과가 없는 경우 (기본값)
            string scoreText = "0";
            string rankImagePath = GetRankImagePath(0);
            AudianceData.EAudianceFeeling feeling = AudianceData.EAudianceFeeling.Bad;
            bool isReputationPositive = false;

            resultHUD.Init(rankImagePath, scoreText, albumNth.ToString(), currentSong.title, feeling, isReputationPositive);
        }
    }

    private System.Collections.IEnumerator WaitForCountdownAndStartMusic()
    {
        // StageFadeInSystemUI의 delayTime (3초) 동안 대기
        const float countdownTime = 3f;
        yield return new WaitForSeconds(countdownTime);

        // 카운트다운 종료 후 음악 재생 시작
        isWaitingForCountdown = false;
        if (audioSource != null && audioSource.clip != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }

        countdownCoroutine = null;
    }

    private string GetRankImagePath(int score)
    {
        // 점수에 따라 랭크 이미지 경로 반환
        // TODO: 실제 랭크 이미지 경로 규칙에 맞게 수정 필요
        if (score >= 90) return "Sprites/Rank/S";
        else if (score >= 80) return "Sprites/Rank/A";
        else if (score >= 70) return "Sprites/Rank/B";
        else if (score >= 60) return "Sprites/Rank/C";
        else return "Sprites/Rank/D";
    }

    private string FormatSongTime(float currentTime, float totalTime)
    {
        int currentMin = Mathf.FloorToInt(currentTime / 60f);
        int currentSec = Mathf.FloorToInt(currentTime % 60f);
        int totalMin = Mathf.FloorToInt(totalTime / 60f);
        int totalSec = Mathf.FloorToInt(totalTime % 60f);
        return $"{currentMin:D2}:{currentSec:D2} / {totalMin:D2}:{totalSec:D2}";
    }

    private void CloseHUDIfExists(UIHUD hud)
    {
        if (hud != null && hud.gameObject != null)
        {
            UIManager.Instance.HUDList.Remove(hud);
            UnityEngine.Object.Destroy(hud.gameObject);
        }
    }

    public void FinishStage()
    {
        Debug.Log("[ComebackScene] Finish");
        OnFinished?.Invoke();
    }

    private void UpdatePractice()
    {
        AudioClip practiceClip = currentSong?.GetPracticeClip();
        if (audioSource == null || currentSong == null || practiceClip == null)
            return;

        // timestamp 업데이트
        practiceSongTimestamp += Time.deltaTime;

        // 음악 종료 체크
        if (!audioSource.isPlaying || practiceSongTimestamp >= practiceClip.length)
        {
            // 음악이 끝나면 정지
            if (audioSource.isPlaying)
                audioSource.Stop();
            isPracticePlaying = false;
            practiceSongTimestamp = 0f;
        }

        // PracticeHUD 업데이트
        if (practiceHUD != null)
        {
            float songLength = practiceClip.length;

            // InitSongSlider: 0-1 사이 값으로 변환
            float sliderValue = songLength > 0 ? Mathf.Clamp01(practiceSongTimestamp / songLength) : 0f;
            practiceHUD.InitSongSlider(sliderValue);

            // InitSongMMSS: MM:SS 형태로 변환
            string mmss = FormatPracticeTime(practiceSongTimestamp);
            practiceHUD.InitSongMMSS(mmss);

            // TODO: InitLyricsText - 가사가 한줄한줄 바뀔 때마다 호출
            // 현재 timestamp에 맞는 가사를 찾아서 표시해야 합니다.
            // 예시: practiceHUD.InitLyricsText(GetLyricsAtTime(practiceSongTimestamp));
            // 일단 제목으로 표시
            practiceHUD.InitLyricsText(currentSong.title);
        }
    }

    private string FormatPracticeTime(float time)
    {
        int min = Mathf.FloorToInt(time / 60f);
        int sec = Mathf.FloorToInt(time % 60f);
        return $"{min:D2}:{sec:D2}";
    }

    private string FormatTime()
    {
        int remain = Mathf.Max(0, (int)(PRACTICE_DURATION - timer));
        return $"{remain / 60:D2}:{remain % 60:D2}";
    }

    private void OnDestroy()
    {
        if (noticeHUD != null)
            noticeHUD.OnCombackPrepareStart -= OnPrepareSignal;

        if (mainMenuHUD != null)
        {
            mainMenuHUD.ClickedPracticeButton -= EnterPractice;
            mainMenuHUD.ClickedComebackButton -= EnterStage;
        }

        if (practiceHUD != null)
        {
            practiceHUD.onClickedPracticeButton -= StartPractice;
            practiceHUD.onClickedExitButton -= ExitPractice;
        }

        // 코루틴 정지
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }

        // AudioSource 정리
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            // AudioSource 컴포넌트 제거
            UnityEngine.Object.Destroy(audioSource);
            audioSource = null;
        }

        // VocalJudge 이벤트 구독 해제
        if (vocalJudge != null)
        {
            vocalJudge.OnFinished -= OnVocalJudgeFinished;
            vocalJudge = null;
        }
    }
}
