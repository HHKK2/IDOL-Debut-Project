using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Popup_Fullscreen : UIPopup
{
    enum Buttons
    {
        CloseButton  // 전체 화면 버튼 또는 "터치해서 넘어가기" 버튼
    }
    
    public Action OnClickClose;

    private Button CloseButton;
    
    private void Start()
    {
        base.Init();
        
        Bind<Button>(typeof(Buttons));
        CloseButton = Get<Button>((int)Buttons.CloseButton);
        BindEvent(CloseButton.gameObject, OnClicked_CloseButton, GameEvents.UIEvent.Click);
    }

    private void OnClicked_CloseButton(PointerEventData eventData)
    {
        OnClickClose?.Invoke();
    }
}