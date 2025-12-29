using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Popup_NewGame : UIPopup
{
    enum Buttons
    {
        Popup_NewGame_Yes_Button,
        Popup_NewGame_No_Button
    }
    
    public Action OnClick_Popup_NewGame_Yes_Button;
    public Action OnClick_Popup_NewGame_No_Button;


    private Button Popup_NewGame_Yes_Button;
    private Button Popup_NewGame_No_Button;
    private void Start()
    {
        base.Init();
        
        Bind<Button>(typeof(Buttons));
        Popup_NewGame_Yes_Button =  Get<Button>((int)Buttons.Popup_NewGame_Yes_Button);
        BindEvent(Popup_NewGame_Yes_Button.gameObject, OnClicked_Popup_NewGame_Yes_Button, GameEvents.UIEvent.Click);
        Popup_NewGame_No_Button = Get<Button>((int)Buttons.Popup_NewGame_No_Button);
        BindEvent(Popup_NewGame_No_Button.gameObject, OnClicked_Popup_NewGame_No_Button , GameEvents.UIEvent.Click);
    }

    private void OnClicked_Popup_NewGame_Yes_Button(PointerEventData eventData)
    {
        OnClick_Popup_NewGame_Yes_Button?.Invoke();
    }
    
    private void OnClicked_Popup_NewGame_No_Button(PointerEventData eventData)
    {
        OnClick_Popup_NewGame_No_Button?.Invoke();
    }
}
