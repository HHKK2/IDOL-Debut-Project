using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class HomeHUD : UIHUD
{
    enum Buttons
    {
        NewGameButton,
        LoadButton,
        ExitButton
    }

    
    public Action ClickedNewGameButton;
    public Action ClickedLoadButton;
    public Action ClickedExitButton;

    private bool initialized = false;

    private void Start()
    {
        if (initialized)
        {
            return;
        }
        
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        if (initialized)
        {
            return;
        }

        base.Init();
        
        Bind<Button>(typeof(Buttons));
        GameObject ClickedNewGameButton =  Get<Button>((int)Buttons.NewGameButton).gameObject;
        BindEvent(ClickedNewGameButton,OnClickedNewGameButton, GameEvents.UIEvent.Click);
        GameObject ClickedLoadButton =  Get<Button>((int)Buttons.LoadButton).gameObject;
        if (SaveManager.Instance.HasSave()==false)
        {
            ClickedLoadButton.GetComponent<Button>().interactable = false;
        }
        BindEvent(ClickedLoadButton,OnClickedLoadButton, GameEvents.UIEvent.Click);
        GameObject ClickedExitButton =  Get<Button>((int)Buttons.ExitButton).gameObject;
        BindEvent(ClickedExitButton,OnClickedExitButton, GameEvents.UIEvent.Click);

        initialized = true;
    }
    private void OnClickedNewGameButton(PointerEventData eventData)
    {
        ClickedNewGameButton?.Invoke();
    }
    
    private void OnClickedLoadButton(PointerEventData eventData)
    {
        ClickedLoadButton?.Invoke();
    }
    
    private void OnClickedExitButton(PointerEventData eventData)
    {
        ClickedExitButton?.Invoke();
    }
}
