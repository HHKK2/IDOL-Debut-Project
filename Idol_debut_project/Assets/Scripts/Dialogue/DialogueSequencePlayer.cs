using System;
using System.Linq;
using UnityEngine;

public class DialogueSequencePlayer : MonoBehaviour
{
    [Header("Refs")]
    public DialogueControllerMulti controller;

    private DialogueText dialogue;
    private int index;
    private bool playing;


    public DialogueBackgroundController BackgroundController;
    public bool IsPlaying => playing;
    
    private Action onFinished;

    public void Play(DialogueText dialogue, Action onFinished = null)
    {
        this.dialogue = dialogue;
        this.onFinished = onFinished;

        index = 0;
        playing = true;

        ShowCurrent();
    }

    private void Update()
    {
        if (!playing) return;

        if (Input.GetMouseButtonDown(0))
        {
            Next();
        }
    }

    private void Next()
    {
        index++;
        ShowCurrent();
    }

    private void ShowCurrent()
    {
        if (dialogue == null || controller == null)
        {
            Stop();
            return;
        }

        if (index >= dialogue.paragraphs.Count)
        {
            Stop();
            onFinished?.Invoke();
            return;
        }

        var text = dialogue.paragraphs[index];

        Speaker speaker = index < dialogue.speakers.Count ? dialogue.speakers[index] : null;
        if (speaker == null && text.StartsWith("BG "))
        {
            string key = text.Substring(3).Trim();
            if (BackgroundController != null)
            {
                BackgroundController.SetBackground(key);
            }

            index++;
            ShowCurrent();
            return;
        }
        controller.ShowDialogue(speaker, text);
    }

    private void Stop()
    {
        playing = false;
        if (controller != null) controller.HideAll();
    }
}