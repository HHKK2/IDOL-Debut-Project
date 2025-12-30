using UnityEngine;

public class TutorialSceneController : MonoBehaviour
{
    [Header("대사 컨트롤러")]
    public DialogueControllerMulti dialogueController;
    
    [Header("튜토리얼 대사")]
    public DialogueText tutorialStep1;
    
    private MainMenuHUD mainMenuHUD;
    private Player player;
    private TimeCycleManager time;
    
    private DialogueText currentDialogue;
    private int dialogueIndex = 0;
    private bool waitingForClick = false;
    private bool waitingForButton = false;

    void Start()
    {
        player = GameManager.Instance.player;
        time = GameManager.Instance.time;
        
        // MainMenuHUD 띄우기
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
        fadeUI.FadeIn(1f, () => {StartStep1();});
    }

    void Update()
    {
        if (waitingForButton) return;
        
        if (waitingForClick && Input.GetMouseButtonDown(0))
        {
            ShowNextLine();
        }
    }

    // ==================== Step 1 ====================
    void StartStep1()
    {
        currentDialogue = tutorialStep1;
        dialogueIndex = 0;
        waitingForClick = true;
        ShowNextLine();
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
        
        OnStep1Finished();
    }

    void OnStep1Finished()
    {
        waitingForClick = false;
        dialogueController.HideAll();
        
        // 컴백 버튼만 활성화
        mainMenuHUD.MustComebackStarted();
        
        // 버튼 클릭 대기
        waitingForButton = true;
        mainMenuHUD.ClickedComebackButton += OnComebackClicked;
    }

    void OnComebackClicked()
    {
        mainMenuHUD.ClickedComebackButton -= OnComebackClicked;
        waitingForButton = false;
        
        Debug.Log("컴백 버튼 클릭됨!");
        
        // TODO: Step 2 진행
    }
}