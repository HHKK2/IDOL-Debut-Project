using System;
using Data;
using UnityEngine;

public class IntroFlowController : MonoBehaviour
{
    [Header("대사 컨트롤러 / 데이터")] 
    public DialogueControllerMulti dialogueController;
    public DialogueText dialogueBeforeInput;
    public DialogueText dialogueAfterInput;

    [Header("배경 컨트롤러")] 
    public DialogueBackgroundController backgroundController;
    public string introBgKey = "intro_image";
    
    private InputHUD inputHUD;

    private DialogueText currentDialogue;
    private int index = 0;
    private bool waitingInput = false;
    

    void Start()
    {
        if (backgroundController != null)
        {
            backgroundController.SetBackground(introBgKey);
        }
        currentDialogue = dialogueBeforeInput;
        ShowNextLine();
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
            dialogueController.ShowDialogue(speaker,text);
            return;
        }
        OnDialogueBlockFinished();
    }

    void OnDialogueBlockFinished()
    {
        dialogueController.HideAll();

        if (currentDialogue == dialogueBeforeInput)
        {
            ShowInputHUD();
        }
        else
        {
            GameSceneManager.Instance.ChangeScene(GameScenes.TutorialScene);
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
        
        inputHUD.gameObject.SetActive(true);
        inputHUD.InputActionFinished += OnInputConfirmed;
    }

    void OnInputConfirmed(PlayerInfoData data)
    {
        if (inputHUD != null)
        {
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
}
