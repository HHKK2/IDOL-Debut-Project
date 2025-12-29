using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    public DialogueControllerMulti controller;
    public DialogueText dialogue;

    int index = 0;

    void Start()
    {
        ShowNext();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            ShowNext();
    }

    void ShowNext()
    {
        
        if (index >= dialogue.paragraphs.Count)
        {
            controller.HideAll();
            Debug.Log("대사 끝!");
            return;
        }

        var text = dialogue.paragraphs[index];

        Speaker speaker = null;
        if (index < dialogue.speakers.Count)
            speaker = dialogue.speakers[index];   // 있으면 쓰고
        // 없으면 speaker=null → 플레이어/내레이션 처리

        controller.ShowDialogue(speaker, text);
        index++;
    }
}