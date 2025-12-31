using System;
using System.Diagnostics;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class IntroFlowController : MonoBehaviour
{
    [Header("대사 컨트롤러 / 데이터")] 
    public DialogueControllerMulti dialogueController;
    public DialogueText dialogueBeforeInput;    // 계약서 입력 전 대사
    public DialogueText dialogueAfterInput;     // 계약서 입력 후 컨펌 대사
    public DialogueText dialogueRetryInput;     // 계약서 재입력 대사 (아니오, 선택 시)
    public DialogueText dialogueAfterConfirm;   // 계약서 확정 대사 (예, 선택 시)


    [Header("배경 컨트롤러")] 
    public DialogueBackgroundController backgroundController;
    public string introBgKey = "intro_image";

    
    
    private InputHUD inputHUD;
    private ConfirmHUD confirmHUD;

    private DialogueText currentDialogue;
    private int index = 0;

    private bool waitingInput = false;
    private bool waitingConfirm = false;


    void Start()
    {
        if (dialogueController != null)
        {
            dialogueController.HideAll();
        }
        if (backgroundController != null)
        {
            backgroundController.SetBackground(introBgKey);
        }
        var fadeUI = UIManager.Instance.ShowSystemUI<FadeInEffectSystemUI>(GameConstants.UI.SystemName.FadeInEffectSystemUI);

        fadeUI.FadeIn(1.5f, () => {
            currentDialogue = dialogueBeforeInput;
            ShowNextLine();
        });
    }

    private void Update()
    {
        if (waitingInput) return;
        if (Input.GetMouseButtonDown(0))
        {
            ShowNextLine();
        }
    }

    void ShowNextLine()
    {
        if (currentDialogue == null)return;
        while (index <currentDialogue.paragraphs.Count)
        {
            var speaker = currentDialogue.speakers[index];
            var text = currentDialogue.paragraphs[index];
            index++;
            if (speaker == null && text.StartsWith("[BG]"))
            {
                if (backgroundController != null)
                {
                    string key = text.Substring(4).Trim();
                    backgroundController.SetBackground(key);
                }
                continue;
            }
            if (currentDialogue == dialogueAfterInput && index >= currentDialogue.paragraphs.Count)
            {
                dialogueController.ShowDialogue(speaker, text, null, () => { ShowConfirmHUD(); });
            }
            else
            {
                dialogueController.ShowDialogue(speaker, text);
            }
            return;
        }
        OnDialogueBlockFinished();
    }

    void OnDialogueBlockFinished()
    {
        if (currentDialogue == dialogueAfterInput)
        {
            return;
        }

        dialogueController.HideAll();

        if (currentDialogue == dialogueBeforeInput)
        {
            ShowInputHUD();
        }
        else if (currentDialogue == dialogueRetryInput)
        {
            ShowInputHUD();
        }
        else if (currentDialogue == dialogueAfterConfirm)
        {
            var fadeUI = UIManager.Instance.ShowSystemUI<FadeInEffectSystemUI>(GameConstants.UI.SystemName.FadeInEffectSystemUI);
            fadeUI.FadeOut(1f, () => {
                GameSceneManager.Instance.ChangeScene(GameScenes.TutorialScene);
            });
        }
    }

    void ShowInputHUD()
    {
        waitingInput = true;
        
        inputHUD = UIManager.Instance.HUDList.Find(h => h is InputHUD) as InputHUD;
        if (inputHUD == null)
        {
            inputHUD = UIManager.Instance.ShowHUDUI<InputHUD>(GameConstants.UI.HUDName.InputHUD);
        }
        inputHUD.InputActionFinished += OnInputConfirmed;
        inputHUD.gameObject.SetActive(true);
    }

    void OnInputConfirmed(PlayerInfoData data)
    {
        if (inputHUD != null)
        {
            UnityEngine.Debug.Log($"받은 성별: {data.gender}");
            inputHUD.InputActionFinished -= OnInputConfirmed;
            UIManager.Instance.CloseHUDUI(GameConstants.UI.HUDName.InputHUD);
            inputHUD = null;
        }

        waitingInput = false;
        GameManager.Instance.player.ApplyPlayerInfo(data);

        if (backgroundController != null)
        {
            backgroundController.SetBackground(introBgKey);
        }

        currentDialogue = dialogueAfterInput;
        index = 0;
        ShowNextLine();
    }


    void ShowConfirmHUD()
    {
        waitingConfirm = true;

        confirmHUD = UIManager.Instance.HUDList.Find(h => h is ConfirmHUD) as ConfirmHUD;
        if (confirmHUD == null)
        {
            confirmHUD = UIManager.Instance.ShowHUDUI<ConfirmHUD>(GameConstants.UI.HUDName.ConfirmHUD);
        }
        confirmHUD.OnClickedYes += OnClickYes;
        confirmHUD.OnClickedNo += OnClickNo;
        confirmHUD.gameObject.SetActive(true);
    }

    void CloseConfirmHUD()
    {
        if (confirmHUD != null)
        {
            confirmHUD.OnClickedYes -= OnClickYes;
            confirmHUD.OnClickedNo -= OnClickNo;
            if (confirmHUD.gameObject != null)
            {
                UIManager.Instance.CloseHUDUI(GameConstants.UI.HUDName.ConfirmHUD);
            }
        }
        confirmHUD = null;
        waitingConfirm = false;
    }

    void OnClickYes()
    {
        CloseConfirmHUD();
        dialogueController.HideAll();

        currentDialogue = dialogueAfterConfirm;
        index = 0;
        ShowNextLine();
    }

    void OnClickNo()
    {
        CloseConfirmHUD();
        dialogueController.HideAll();

        currentDialogue = dialogueRetryInput;
        index = 0;
        ShowNextLine();
    }
}
