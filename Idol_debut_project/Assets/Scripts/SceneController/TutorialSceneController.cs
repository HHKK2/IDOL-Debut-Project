using UnityEngine;
using UnityEngine.SceneManagement;
using Data;
using UnityEngine.UI;

public class TutorialSceneController : MonoBehaviour
{
    private enum TutorialSteps
    {
        Comeback,
        Practice,
        Debut,
        StartStage,
        Result,
        ResultClick,
        Mentality,
        Rest,
        Date,
        Fanservice,
        Calendar
    }

    [Header("대사 컨트롤러")]
    public DialogueControllerMulti dialogueController;

    [Header("튜토리얼 대사")]
    public DialogueText comebackNarration;
    public DialogueText practiceNarration;
    public DialogueText debutNarration;
    public DialogueText startStageNarration;
    public DialogueText resultNarration;
    public DialogueText mentalityNarration;
    public DialogueText restNarration;
    public DialogueText dateNarration;
    public DialogueText fanserviceNarration;
    public DialogueText calendarNarration;

    [Header("컴백 시나리오")]
    public ComebackScenarioSO comebackScenario;

    [Header("노래방 가사 플레이어")]
    [SerializeField] private KaraokeLinePlayer karaokePlayer;

    // HUD 참조
    private MainMenuHUD mainMenuHUD;
    private CombackNoticeHUD combackNoticeHUD;
    private TimerHUD timerHUD;
    private PracticeHUD practiceHUD;
    private StageHUD stageHUD;
    private StageResultHUD stageResultHUD;

    // 오디오
    private AudioSource audioSource;

    // 플레이어/시간 참조
    private Player player;
    private TimeCycleManager time;

    // 다이얼로그 상태
    private DialogueText currentDialogue;
    private int dialogueIndex = 0;
    private bool hasSeenPracticeDialogue = false;
    private bool isPracticePhaseComplete = false;
    private TutorialSteps currentStep;
    private bool waitingForClick = false;
    private bool waitingForButton = false;

    // 연습/무대 재생 상태
    private bool isPracticePlaying = false;
    private bool isStagePlaying = false;
    private float songTimestamp = 0f;
    private float clipLength = 0f;

    // 무대용 가사 데이터
    private KaraokeSongData stageKaraokeData;
    private int currentStageLineIndex = -1;

    // ★ VocalJudge 채점 관련 ★
    private VocalJudge vocalJudge;
    private VocalResult vocalResult;

    void Start()
    {
        player = GameManager.Instance.player;
        time = GameManager.Instance.time;
        time.SetTutorial(true);

        // AudioSource 초기화
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;

        mainMenuHUD = UIManager.Instance.HUDList.Find(h => h is MainMenuHUD) as MainMenuHUD;
        if (mainMenuHUD == null)
        {
            mainMenuHUD = UIManager.Instance.ShowHUDUI<MainMenuHUD>();
        }

        mainMenuHUD.Init(
            date: time.GetCurrentDateString(),
            groupName: player.GroupName,
            fanNum: player.FanNumber.ToString(),
            mental: player.GetMentalRatio(),
            name: player.Name
        );

        if (dialogueController != null)
        {
            dialogueController.HideAll();
        }

        var fadeUI = UIManager.Instance.ShowSystemUI<FadeInEffectSystemUI>(GameConstants.UI.SystemName.FadeInEffectSystemUI);
        fadeUI.FadeIn(1f, () => { StartComebackIntro(); });
    }

    void Update()
    {
        if (waitingForButton) return;

        if (waitingForClick && Input.GetMouseButtonDown(0))
        {
            ShowNextLine();
        }

        // 연습 중 업데이트
        if (isPracticePlaying)
        {
            UpdatePractice();
        }

        // 무대 중 업데이트
        if (isStagePlaying)
        {
            UpdateStage();
        }

        // 테스트: Space로 스킵
        if ((isPracticePlaying || isStagePlaying) && Input.GetKeyDown(KeyCode.Space))
        {
            if (isPracticePlaying) EndPracticeAndReturnToMenu();
            else if (isStagePlaying) EndStage();
        }
    }

    #region 다이얼로그 시스템

    void ShowNextLine()
    {
        // ★ currentDialogue 없으면 바로 OnDialogueFinished() ★
        if (currentDialogue == null)
        {
            OnDialogueFinished();
            return;
        }

        if (dialogueIndex < currentDialogue.paragraphs.Count)
        {
            var speaker = currentDialogue.speakers[dialogueIndex];
            var text = currentDialogue.paragraphs[dialogueIndex];
            dialogueIndex++;

            dialogueController.ShowDialogue(speaker, text);
            return;
        }
        OnDialogueFinished();
    }

    void OnDialogueFinished()
    {
        waitingForClick = false;
        dialogueController.HideAll();

        switch (currentStep)
        {
            case TutorialSteps.Comeback:
                mainMenuHUD.MustComebackStarted();
                waitingForButton = true;
                mainMenuHUD.ClickedComebackButton += OnComebackClicked;
                break;

            case TutorialSteps.Practice:
                mainMenuHUD.ResetActionButtons();
                mainMenuHUD.MustPracticeStarted();
                waitingForButton = true;
                mainMenuHUD.ClickedPracticeButton -= OnPracticeClicked;
                mainMenuHUD.ClickedPracticeButton += OnPracticeClicked;
                break;

            case TutorialSteps.Debut:
                // 대사 끝나면 그냥 대기 (연습 버튼 이미 눌린 상태)
                break;

            case TutorialSteps.StartStage:
                mainMenuHUD.ResetActionButtons();
                mainMenuHUD.MustComebackStarted();
                waitingForButton = true;
                mainMenuHUD.ClickedComebackButton += OnStageStartClicked;
                break;
            case TutorialSteps.Result:
                Debug.Log("Result 케이스 진입!");
                
                // ★ StageResultHUD 유지한 채로 클릭 대기 ★
                currentDialogue = null;
                currentStep = TutorialSteps.ResultClick;
                waitingForClick = true;
                Debug.Log("[Tutorial] 결과 확인 후 클릭하면 메인메뉴로 이동");
                break;

            case TutorialSteps.ResultClick:
                // ★ 여기서 HUD 닫기 ★
                if (stageResultHUD != null)
                {
                    UIManager.Instance.CloseHUDUI(GameConstants.UI.HUDName.StageResultHUD);
                    stageResultHUD = null;
                }
                try { UIManager.Instance.CloseHUDUI("TwitterHUD"); } catch { }
                
                StartMentalityTutorial();
                break;

            case TutorialSteps.Mentality:
                StartRestTutorial();
                break;

            case TutorialSteps.Rest:
                StartDateTutorial();
                break;

            case TutorialSteps.Date:
                StartFanserviceTutorial();
                break;

            case TutorialSteps.Fanservice:
                StartCalendarTutorial();
                break;

            case TutorialSteps.Calendar:
                OnTutorialComplete();
                break;
        }
    }

    #endregion

    #region 컴백 인트로

    void StartComebackIntro()
    {
        currentStep = TutorialSteps.Comeback;
        currentDialogue = comebackNarration;
        dialogueIndex = 0;
        waitingForClick = true;
        ShowNextLine();
    }

    void OnComebackClicked()
    {
        mainMenuHUD.ClickedComebackButton -= OnComebackClicked;
        waitingForButton = false;
        currentStep = TutorialSteps.Practice;

        var song = comebackScenario.GetSong(0, player.Gender);
        combackNoticeHUD = UIManager.Instance.ShowHUDUI<CombackNoticeHUD>(GameConstants.UI.HUDName.CombackNoticeHUD);
        combackNoticeHUD.InitWithSprite(
            albumCover: song.albumCover,
            conceptName: song.concept,
            songName: song.title
        );

        combackNoticeHUD.OnCombackPrepareStart += OnComebackNoticeClicked;
    }

    void OnComebackNoticeClicked()
    {
        combackNoticeHUD.OnCombackPrepareStart -= OnComebackNoticeClicked;
        UIManager.Instance.CloseHUDUI(GameConstants.UI.HUDName.CombackNoticeHUD);
        combackNoticeHUD = null;

        StartPracticeIntro();
    }

    #endregion

    #region 연습 Phase

    void StartPracticeIntro()
    {
        currentStep = TutorialSteps.Practice;
        timerHUD = UIManager.Instance.ShowHUDUI<TimerHUD>(GameConstants.UI.HUDName.TimerHUD);

        timerHUD.StartCountdown(10f); // 10분
        timerHUD.OnTimerFinished += OnTimerEnd;

        currentDialogue = practiceNarration;
        dialogueIndex = 0;
        waitingForClick = true;
        ShowNextLine();
    }

    void OnPracticeClicked()
    {
        mainMenuHUD.ClickedPracticeButton -= OnPracticeClicked;
        waitingForButton = false;

        currentStep = TutorialSteps.Debut;

        var song = comebackScenario.GetSong(0, player.Gender);

        practiceHUD = UIManager.Instance.ShowHUDUI<PracticeHUD>(GameConstants.UI.HUDName.PracticeHUD);
        practiceHUD.InitWithSprite(
            songTitle: song.title,
            albumCover: song.albumCover
        );
        practiceHUD.SetSelectingMode();

        practiceHUD.onClickedExitButton += OnPracticeExitClicked;
        practiceHUD.onClickedPracticeButton += OnPracticeStartClicked;

        if (!hasSeenPracticeDialogue)
        {
            hasSeenPracticeDialogue = true;
            currentDialogue = debutNarration;
            dialogueIndex = 0;
            waitingForClick = true;
            ShowNextLine();
        }
    }

    void OnPracticeExitClicked()
    {
        EndPractice();

        // PracticeHUD 닫기
        if (practiceHUD != null)
        {
            practiceHUD.onClickedExitButton -= OnPracticeExitClicked;
            practiceHUD.onClickedPracticeButton -= OnPracticeStartClicked;
            UIManager.Instance.CloseHUDUI(GameConstants.UI.HUDName.PracticeHUD);
            practiceHUD = null;
        }

        dialogueController.HideAll();
        mainMenuHUD.MustPracticeStarted();

        waitingForButton = true;
        mainMenuHUD.ClickedPracticeButton -= OnPracticeClicked;
        mainMenuHUD.ClickedPracticeButton += OnPracticeClicked;
    }

    void OnPracticeStartClicked()
    {
        practiceHUD.onClickedPracticeButton -= OnPracticeStartClicked;

        practiceHUD.SetPlayingMode();
        practiceHUD.SetExitButtonActive(false);

        var song = comebackScenario.GetSong(0, player.Gender);

        AudioClip clip = song.GetPracticeClip();
        if (clip == null)
        {
            Debug.LogError("연습용 오디오 클립 연결 확인 요망.");
            return;
        }

        // 오디오 설정
        audioSource.clip = clip;
        clipLength = clip.length;
        songTimestamp = 0f;
        isPracticePlaying = true;

        // KaraokeLinePlayer 초기화 (연습용 - 한 글자씩 가사)
        if (karaokePlayer != null && song.karaokeJsonAsset != null)
        {
            karaokePlayer.lyricsText = practiceHUD.GetLyricsText();
            karaokePlayer.nextLyricsText = practiceHUD.GetNextLyricsText();
            karaokePlayer.Init(audioSource, song.karaokeJsonAsset);
        }

        // 음악 재생
        audioSource.Play();

        Debug.Log($"[Tutorial] 연습 시작! 곡 길이: {clip.length}초");
    }

    void UpdatePractice()
    {
        if (audioSource == null || !isPracticePlaying) return;

        songTimestamp += Time.deltaTime;

        // HUD 업데이트
        if (practiceHUD != null)
        {
            practiceHUD.InitSongMMSS(FormatTime(songTimestamp));
            practiceHUD.InitSongSlider(clipLength > 0 ? songTimestamp / clipLength : 0f);
        }

        // 곡 종료 체크
        if (!audioSource.isPlaying || songTimestamp >= clipLength)
        {
            EndPracticeAndReturnToMenu();
        }
    }

    void EndPractice()
    {
        isPracticePlaying = false;

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        Debug.Log("[Tutorial] 연습 종료");
    }

    void EndPracticeAndReturnToMenu()
    {
        EndPractice();

        // PracticeHUD 닫기
        if (practiceHUD != null)
        {
            practiceHUD.onClickedExitButton -= OnPracticeExitClicked;
            practiceHUD.onClickedPracticeButton -= OnPracticeStartClicked;
            UIManager.Instance.CloseHUDUI(GameConstants.UI.HUDName.PracticeHUD);
            practiceHUD = null;
        }

        // 메인 메뉴로 복귀
        dialogueController.HideAll();
        mainMenuHUD.MustPracticeStarted();

        waitingForButton = true;
        mainMenuHUD.ClickedPracticeButton -= OnPracticeClicked;
        mainMenuHUD.ClickedPracticeButton += OnPracticeClicked;
    }

    void OnTimerEnd()
    {
        timerHUD.OnTimerFinished -= OnTimerEnd;

        UIManager.Instance.CloseHUDUI(GameConstants.UI.HUDName.TimerHUD);
        timerHUD = null;

        waitingForButton = false;
        waitingForClick = false;

        mainMenuHUD.ClickedPracticeButton -= OnPracticeClicked;
        mainMenuHUD.ClickedComebackButton -= OnComebackClicked;

        // ★ 연습 중이면 정지 ★
        if (isPracticePlaying)
        {
            EndPractice();
        }

        // ★ PracticeHUD 무조건 닫기 (연습 중이든 아니든) ★
        if (practiceHUD != null)
        {
            practiceHUD.onClickedExitButton -= OnPracticeExitClicked;
            practiceHUD.onClickedPracticeButton -= OnPracticeStartClicked;
            UIManager.Instance.CloseHUDUI(GameConstants.UI.HUDName.PracticeHUD);
            practiceHUD = null;
        }

        // ★ CombackNoticeHUD도 닫기 ★
        if (combackNoticeHUD != null)
        {
            combackNoticeHUD.OnCombackPrepareStart -= OnComebackNoticeClicked;
            UIManager.Instance.CloseHUDUI(GameConstants.UI.HUDName.CombackNoticeHUD);
            combackNoticeHUD = null;
        }

        // MainMenuHUD 다시 표시
        if (mainMenuHUD == null)
        {
            mainMenuHUD = UIManager.Instance.ShowHUDUI<MainMenuHUD>();
        }
        mainMenuHUD.gameObject.SetActive(true);
        mainMenuHUD.ResetActionButtons();
        mainMenuHUD.MustComebackStarted();

        mainMenuHUD.Init(
            date: time.GetCurrentDateString(),
            groupName: player.GroupName,
            fanNum: player.FanNumber.ToString(),
            mental: player.GetMentalRatio(),
            name: player.Name
        );

        isPracticePhaseComplete = true;

        StartStageIntro();
    }

    #endregion

    #region 무대 Phase

    void StartStageIntro()
    {
        currentStep = TutorialSteps.StartStage;

        dialogueController.gameObject.SetActive(true);
        dialogueController.HideAll();

        mainMenuHUD.ResetActionButtons();
        mainMenuHUD.MustComebackStarted();

        currentDialogue = startStageNarration;
        dialogueIndex = 0;
        waitingForClick = true;
        ShowNextLine();
    }

    void OnStageStartClicked()
    {
        mainMenuHUD.ClickedComebackButton -= OnStageStartClicked;
        waitingForButton = false;

        StartStage();
    }

    void StartStage()
    {
        currentStep = TutorialSteps.Result;

        dialogueController.HideAll();

        // MainMenuHUD 숨기기
        mainMenuHUD.gameObject.SetActive(false);

        // VocalResult 초기화
        vocalResult = null;

        // StageHUD 표시
        stageHUD = UIManager.Instance.ShowHUDUI<StageHUD>(GameConstants.UI.HUDName.StageHUD);

        // 카운트다운 후 시작
        StartCoroutine(StartStageAfterCountdown());
    }

    private System.Collections.IEnumerator StartStageAfterCountdown()
    {
        // StageFadeIn 카운트다운 대기 (3초)
        yield return new WaitForSeconds(3f);

        var song = comebackScenario.GetSong(0, player.Gender);

        AudioClip clip = song.GetComebackClip();
        if (clip == null)
        {
            Debug.LogError("무대용 오디오 클립 연결 확인 요망.");
            yield break;
        }

        // 가사 데이터 로드 (무대용 - 한 줄씩 표시)
        if (song.karaokeJsonAsset != null)
        {
            stageKaraokeData = JsonUtility.FromJson<KaraokeSongData>(song.karaokeJsonAsset.text);
            currentStageLineIndex = -1;
        }

        // 오디오 설정
        audioSource.clip = clip;
        clipLength = clip.length;
        songTimestamp = 0f;
        isStagePlaying = true;

        // ★★★ VocalJudge 초기화 (실제 채점) ★★★
        vocalJudge = stageHUD != null ? stageHUD.GetComponentInChildren<VocalJudge>(true) : null;
        if (vocalJudge != null)
        {
            vocalJudge.songAudioSource = audioSource;
            vocalJudge.useAudioSourceTime = true;
            vocalJudge.OnFinished += OnVocalJudgeFinished;
            vocalJudge.OnAudienceFeelingUpdated += OnAudienceFeelingUpdated;

            // ScoreChart 초기화
            if (vocalJudge.scoreChart != null && song.scoreChartJsonAsset != null)
            {
                vocalJudge.scoreChart.Init(song.scoreChartJsonAsset);
            }

            Debug.Log("[Tutorial] VocalJudge 연결 완료! 실제 채점 시작");
        }
        else
        {
            Debug.LogWarning("[Tutorial] VocalJudge를 찾을 수 없습니다. 기본 점수 사용됩니다.");
        }

        // KaraokeLinePlayer 연결 (StageHUD 자식에 있으면)
        var karaoke = stageHUD != null ? stageHUD.GetComponentInChildren<KaraokeLinePlayer>(true) : null;
        if (karaoke != null)
        {
            karaoke.Init(audioSource, song.karaokeJsonAsset);
        }

        // 음악 재생
        audioSource.Play();

        Debug.Log($"[Tutorial] 무대 시작! 곡 길이: {clip.length}초");
    }

    void OnVocalJudgeFinished(VocalResult result)
    {
        vocalResult = result;
        Debug.Log($"[Tutorial] VocalJudge 완료! 점수: {result.finalScore100}");
    }

    void OnAudienceFeelingUpdated(AudianceData.EAudianceFeeling feeling)
    {
        if (stageHUD != null)
        {
            stageHUD.InitAudianceImage(feeling);
        }
    }

    void UpdateStage()
    {
        if (audioSource == null || !isStagePlaying) return;

        songTimestamp += Time.deltaTime;

        // StageHUD 업데이트
        if (stageHUD != null)
        {
            float sliderValue = clipLength > 0 ? Mathf.Clamp01(songTimestamp / clipLength) : 0f;
            string timerText = FormatSongTime(songTimestamp, clipLength);
            stageHUD.InitSongTimerValue(sliderValue, timerText);

            // 가사 업데이트 (한 줄씩) - KaraokeLinePlayer가 없을 때 fallback
            UpdateStageLyrics(songTimestamp);
        }

        // 곡 종료 체크
        if (!audioSource.isPlaying || songTimestamp >= clipLength)
        {
            EndStage();
        }
    }

    void UpdateStageLyrics(float currentTime)
    {
        if (stageKaraokeData == null || stageKaraokeData.lines == null) return;

        var line = stageKaraokeData.GetLineAtTime(currentTime);
        if (line == null) return;

        if (currentStageLineIndex != line.line_index)
        {
            currentStageLineIndex = line.line_index;

            string nextText = "";
            if (line.line_index + 1 < stageKaraokeData.lines.Count)
            {
                nextText = stageKaraokeData.lines[line.line_index + 1].text;
            }

            stageHUD.InitLyricsText(line.text, nextText);
        }
    }

    void EndStage()
    {
        if (!isStagePlaying) return;

        isStagePlaying = false;

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        OnStageComplete();
    }

    void OnStageComplete()
    {
        // StageHUD 닫기
        if (stageHUD != null)
        {
            UIManager.Instance.CloseHUDUI(GameConstants.UI.HUDName.StageHUD);
            stageHUD = null;
        }

        currentStep = TutorialSteps.Result;

        var song = comebackScenario.GetSong(0, player.Gender);

        // ★★★ 실제 채점 결과 사용 ★★★
        int finalScore;
        if (vocalResult != null)
        {
            finalScore = vocalResult.finalScore100;
            Debug.Log($"[Tutorial] 실제 점수 사용: {finalScore}");
        }
        else
        {
            finalScore = 50; // VocalJudge 없으면 기본값
            Debug.LogWarning("[Tutorial] VocalResult 없음, 기본 점수 50 사용");
        }

        string rankImagePath = GetRankImagePath(finalScore);
        AudianceData.EAudianceFeeling feeling = AudianceFeelingUtil.FromScore100(finalScore);
        bool isReputationPositive = finalScore >= 50;

        stageResultHUD = UIManager.Instance.ShowHUDUI<StageResultHUD>(GameConstants.UI.HUDName.StageResultHUD);
        stageResultHUD.Init(
            rankImagePath: rankImagePath,
            scoreText: finalScore.ToString(),
            albumNth: "1",
            songName: song.title,
            audianceFeeling: feeling,
            isReputationPositiveNumber: isReputationPositive
        );

        Debug.Log($"[Tutorial] 무대 완료! 최종 점수: {finalScore}");


        // ★ FadeInEffectSystemUI 닫기 ★
        try
        {
            UIManager.Instance.CloseSystemUI(GameConstants.UI.SystemName.FadeInEffectSystemUI);
            Debug.Log("[Tutorial] FadeInEffectSystemUI 닫음");
        }
        catch { }

        // 다이얼로그 띄우기
        if (dialogueController != null)
        {
            dialogueController.gameObject.SetActive(true);
        }

        currentDialogue = resultNarration;
        dialogueIndex = 0;
        waitingForClick = true;
        ShowNextLine();
    }

    void OnStageResultConfirmed()
    {
        Debug.Log("[Tutorial] 퇴근 버튼 클릭됨!");

        stageResultHUD.OnClickGoHomeButton -= OnStageResultConfirmed;

        // HUD 닫기
        UIManager.Instance.CloseHUDUI(GameConstants.UI.HUDName.StageResultHUD);
        stageResultHUD = null;

        // TwitterHUD도 닫기
        try
        {
            UIManager.Instance.CloseHUDUI("TwitterHUD");
        }
        catch { }

        waitingForButton = false;

        StartMentalityTutorial();
    }

    #endregion

    #region 후반 튜토리얼 (멘탈/휴식/연애/팬서비스/캘린더)

    void StartMentalityTutorial()
    {
        currentStep = TutorialSteps.Mentality;

        if (dialogueController != null)
        {
            dialogueController.gameObject.SetActive(true);
        }

        if (mainMenuHUD == null)
        {
            mainMenuHUD = UIManager.Instance.ShowHUDUI<MainMenuHUD>();
        }
        mainMenuHUD.gameObject.SetActive(true);

        mainMenuHUD.Init(
            date: time.GetCurrentDateString(),
            groupName: player.GroupName,
            fanNum: player.FanNumber.ToString(),
            mental: player.GetMentalRatio(),
            name: player.Name
        );

        mainMenuHUD.ResetActionButtons();

        currentDialogue = mentalityNarration;
        dialogueIndex = 0;
        waitingForClick = true;
        ShowNextLine();
    }

    void StartRestTutorial()
    {
        currentStep = TutorialSteps.Rest;

        mainMenuHUD.ResetActionButtons();

        currentDialogue = restNarration;
        dialogueIndex = 0;
        waitingForClick = true;
        ShowNextLine();
    }

    void StartDateTutorial()
    {
        currentStep = TutorialSteps.Date;

        mainMenuHUD.ResetActionButtons();

        currentDialogue = dateNarration;
        dialogueIndex = 0;
        waitingForClick = true;
        ShowNextLine();
    }

    void StartFanserviceTutorial()
    {
        currentStep = TutorialSteps.Fanservice;

        mainMenuHUD.ResetActionButtons();

        currentDialogue = fanserviceNarration;
        dialogueIndex = 0;
        waitingForClick = true;
        ShowNextLine();
    }

    void StartCalendarTutorial()
    {
        currentStep = TutorialSteps.Calendar;

        mainMenuHUD.ResetActionButtons();

        currentDialogue = calendarNarration;
        dialogueIndex = 0;
        waitingForClick = true;
        ShowNextLine();
    }

    void OnTutorialComplete()
    {
        Debug.Log("[Tutorial] 튜토리얼 완료! HomeScene으로 전환");
        time.SetTutorial(false);
        SceneManager.LoadScene(GameScenes.HomeScene);
    }

    #endregion

    #region 유틸리티

    private string FormatTime(float time)
    {
        int m = (int)(time / 60);
        int s = (int)(time % 60);
        return $"{m:00}:{s:00}";
    }

    private string FormatSongTime(float currentTime, float totalTime)
    {
        int currentMin = Mathf.FloorToInt(currentTime / 60f);
        int currentSec = Mathf.FloorToInt(currentTime % 60f);
        int totalMin = Mathf.FloorToInt(totalTime / 60f);
        int totalSec = Mathf.FloorToInt(totalTime % 60f);
        return $"{currentMin:D2}:{currentSec:D2} / {totalMin:D2}:{totalSec:D2}";
    }

    private string GetRankImagePath(int score)
    {
        if (score >= 90) return "Sprites/Rank/StageResultS";
        else if (score >= 60) return "Sprites/Rank/StageResultA";
        else if (score >= 40) return "Sprites/Rank/StageResultB";
        else if (score >= 10) return "Sprites/Rank/StageResultC";
        else return "Sprites/Rank/StageResultF";
    }

    #endregion

    #region 정리

    private void OnDestroy()
    {
        // 이벤트 해제
        if (mainMenuHUD != null)
        {
            mainMenuHUD.ClickedComebackButton -= OnComebackClicked;
            mainMenuHUD.ClickedComebackButton -= OnStageStartClicked;
            mainMenuHUD.ClickedPracticeButton -= OnPracticeClicked;
        }

        if (combackNoticeHUD != null)
        {
            combackNoticeHUD.OnCombackPrepareStart -= OnComebackNoticeClicked;
        }

        if (practiceHUD != null)
        {
            practiceHUD.onClickedExitButton -= OnPracticeExitClicked;
            practiceHUD.onClickedPracticeButton -= OnPracticeStartClicked;
        }

        if (timerHUD != null)
        {
            timerHUD.OnTimerFinished -= OnTimerEnd;
        }

        if (stageResultHUD != null)
        {
            stageResultHUD.OnClickGoHomeButton -= OnStageResultConfirmed;
        }

        // ★ VocalJudge 정리 ★
        if (vocalJudge != null)
        {
            vocalJudge.OnFinished -= OnVocalJudgeFinished;
            vocalJudge.OnAudienceFeelingUpdated -= OnAudienceFeelingUpdated;
            vocalJudge = null;
        }

        // 오디오 정리
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            Destroy(audioSource);
            audioSource = null;
        }
    }

    #endregion
}