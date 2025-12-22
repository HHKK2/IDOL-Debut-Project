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

    enum GameObjects
    {
        Main,
        InputInfo,
        
    }
    
    public Action ClickedNewGameButton;
    public Action ClickedLoadButton;
    public Action ClickedExitButton;


    private void Start()
    {
        base.Init();
        
        Bind<Button>(typeof(Buttons));
        GameObject ClickedNewGameButton =  Get<Button>((int)Buttons.NewGameButton).gameObject;
        BindEvent(ClickedNewGameButton,OnClickedNewGameButton, GameEvents.UIEvent.Click);
        GameObject ClickedLoadButton =  Get<Button>((int)Buttons.LoadButton).gameObject;
        BindEvent(ClickedLoadButton,OnClickedLoadButton, GameEvents.UIEvent.Click);
        GameObject ClickedExitButton =  Get<Button>((int)Buttons.ExitButton).gameObject;
        BindEvent(ClickedExitButton,OnClickedExitButton, GameEvents.UIEvent.Click);
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
