using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HomeUI : UIHUD
{
    enum Texts
    {
        FanText,
        NameText,
        GroupNameText,
        DateText
    }

    enum Sliders
    {
        MentalSlider
    }

    enum Buttons
    {
        LiveButton,
        PracticeButton,
        DatingButton,
        RestButton,
        ComebackButton,
        SaveButton,
        SettingButton,
        ExitButton
    }

    public Action ClickedLiveButton;
    public Action ClickedPracticeButton;
    public Action ClickedDatingButton;
    public Action ClickedRestButton;
    public Action ClickedComebackButton;
    public Action ClickedSaveButton;
    public Action ClickedSettingButton;
    public Action ClickedExitButton;

    

    private TextMeshProUGUI FanText;
    private TextMeshProUGUI NameText;
    private TextMeshProUGUI GroupNameText;
    private TextMeshProUGUI DateText;

    private Slider MentalSlider;

    private void Start()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(Texts));
        FanText = Get<TextMeshProUGUI>((int)Texts.FanText);
        NameText = Get<TextMeshProUGUI>((int)Texts.NameText);
        GroupNameText = Get<TextMeshProUGUI>((int)Texts.GroupNameText);
        DateText = Get<TextMeshProUGUI>((int)Texts.DateText);
        
        Bind<Slider>(typeof(Sliders));
        MentalSlider = Get<Slider>((int)Sliders.MentalSlider);
        
        Bind<Button>(typeof(Buttons));
        GameObject LiveButton =  Get<Button>((int)Buttons.LiveButton).gameObject;
        BindEvent(LiveButton,OnClickedLiveButton, GameEvents.UIEvent.Click);
        GameObject PracticeButton =  Get<Button>((int)Buttons.PracticeButton).gameObject;
        BindEvent(PracticeButton,OnClickedPracticeButton, GameEvents.UIEvent.Click);
        GameObject DatingButton =  Get<Button>((int)Buttons.DatingButton).gameObject;
        BindEvent(DatingButton,OnClickedDatingButton, GameEvents.UIEvent.Click);
        GameObject RestButton =  Get<Button>((int)Buttons.RestButton).gameObject;
        BindEvent(RestButton,OnClickedRestButton, GameEvents.UIEvent.Click);
        GameObject ComebackButton =  Get<Button>((int)Buttons.ComebackButton).gameObject;
        BindEvent(ComebackButton,OnClickedComebackButton, GameEvents.UIEvent.Click);
        GameObject SaveButton =  Get<Button>((int)Buttons.SaveButton).gameObject;
        BindEvent(SaveButton,OnClickedSaveButton, GameEvents.UIEvent.Click);
        GameObject SettingButton =  Get<Button>((int)Buttons.SettingButton).gameObject;
        BindEvent(SettingButton,OnClickedSettingButton, GameEvents.UIEvent.Click);
        GameObject ExitButton =  Get<Button>((int)Buttons.ExitButton).gameObject;
        BindEvent(ExitButton,OnClickedExitButton, GameEvents.UIEvent.Click);
    }

    private void OnClickedLiveButton(PointerEventData eventData)
    {
        ClickedLiveButton?.Invoke();
    }
    
    private void OnClickedPracticeButton(PointerEventData eventData)
    {
        ClickedPracticeButton?.Invoke();
    }
    
    private void OnClickedDatingButton(PointerEventData eventData)
    {
        ClickedDatingButton?.Invoke();
    }
    
    private void OnClickedRestButton(PointerEventData eventData)
    {
        ClickedRestButton?.Invoke();
    }
    
    private void OnClickedComebackButton(PointerEventData eventData)
    {
        ClickedComebackButton?.Invoke();
    }

    private void OnClickedSaveButton(PointerEventData eventData)
    {
        ClickedSaveButton?.Invoke();
    }

    private void OnClickedSettingButton(PointerEventData eventData)
    {
        ClickedSettingButton?.Invoke();
    }
    
    private void OnClickedExitButton(PointerEventData eventData)
    {
        ClickedExitButton?.Invoke();
    }

    /// <summary>
    /// stat Visual 초기화 함수
    /// </summary>
    /// <param name="mental"> 0-1사이값으로 넣기</param>
    public void Init(string date, string groupName, string fanNum, float mental, string name)
    {
        FanText.text = groupName;
        NameText.text = name;
        GroupNameText.text = groupName;
        DateText.text = fanNum;
        MentalSlider.value = mental;
    }
}
