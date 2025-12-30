using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class ConfirmHUD : UIHUD
{
    enum Buttons
    {
        YesButton,
        NoButton
    }

    public Action OnClickedYes;
    public Action OnClickedNo;

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
        
        Button YesButton = Get<Button>((int)Buttons.YesButton);
        BindEvent(YesButton.gameObject, OnClickedYesButton, GameEvents.UIEvent.Click);
        
        Button NoButton = Get<Button>((int)Buttons.NoButton);
        BindEvent(NoButton.gameObject, OnClickedNoButton, GameEvents.UIEvent.Click);

        initialized = true;
    }

    private void OnClickedYesButton(PointerEventData eventData)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        OnClickedYes?.Invoke();
    }

    private void OnClickedNoButton(PointerEventData eventData)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        OnClickedNo?.Invoke();
    }
}