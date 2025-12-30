using UnityEngine;

public class TutorialSceneController : MonoBehaviour
{
    private enum TutorialSteps
    {
        Comeback,
        Practice,
        Debut,
        StartStage,
        Result,
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



    private MainMenuHUD mainMenuHUD;
    private CombackNoticeHUD combackNoticeHUD;
    private TimerHUD timerHUD;
    private PracticeHUD practiceHUD;

    private Player player;
    private TimeCycleManager time;

    private DialogueText currentDialogue;
    private int dialogueIndex = 0;
    private TutorialSteps currentStep;
    private bool waitingForClick = false;
    private bool waitingForButton = false;

    void Start()
    {
        player = GameManager.Instance.player;
        time = GameManager.Instance.time;
        time.SetTutorial(true); //튜토리얼 시작

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
    }


    void ShowNextLine()
    {
        if (currentDialogue == null) return;

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
                mainMenuHUD.ClickedPracticeButton += OnPracticeClicked;
                break;
            case TutorialSteps.Debut:
                time.SetTutorial(false); //튜토리얼이 끝났습니다. TODO : 나중에 실제로 튜토리얼이 끝나는 부분으로 옮겨주시면 됨.
                // 연습 진행...
                // 이후 단계 구현 필요
                break;
            default:
                break;
        }
    }

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

        combackNoticeHUD = UIManager.Instance.ShowHUDUI<CombackNoticeHUD>(GameConstants.UI.HUDName.CombackNoticeHUD);
        combackNoticeHUD.Init(
            albumImagePath: "Sprites/AlbumCovers/AlbumImage",
            conceptName: "몽환",
            songName: "SLEEPWALKING"
        // 이 부분 확인 필요. 튜토리얼 곡과 연결했다고는 하는데....
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

    void StartPracticeIntro()
    {
        currentStep = TutorialSteps.Practice;
        timerHUD = UIManager.Instance.ShowHUDUI<TimerHUD>(GameConstants.UI.HUDName.TimerHUD);
        timerHUD.StartCountdown(600f);
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
        practiceHUD = UIManager.Instance.ShowHUDUI<PracticeHUD>(GameConstants.UI.HUDName.PracticeHUD);
        practiceHUD.Init(
            songTitle: "SLEEPWALKING",
            albumImagePath: "Sprites/AlbumCovers/AlbumImage"
        // 이것도 확인 필요. 연결은 해뒀다는데....
        );
        practiceHUD.SetSelectingMode();

        currentDialogue = debutNarration;
        dialogueIndex = 0;
        waitingForClick = true;
        ShowNextLine();
    }

    void OnTimerEnd()
    {
        timerHUD.OnTimerFinished -= OnTimerEnd;
        Debug.Log("타이머 종료!");
    }
}