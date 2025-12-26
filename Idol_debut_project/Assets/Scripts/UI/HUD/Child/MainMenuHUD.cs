using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuHUD : UIHUD
{
    enum Texts
    {
        FanText,
        NameText,
        GroupNameText,
        DateText,
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



    private Button LiveButton;
    private Button DatingButton;
    private Button RestButton;
    private Button PracticeButton;

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
        Bind<TextMeshProUGUI>(typeof(Texts));
        FanText = Get<TextMeshProUGUI>((int)Texts.FanText);
        NameText = Get<TextMeshProUGUI>((int)Texts.NameText);
        GroupNameText = Get<TextMeshProUGUI>((int)Texts.GroupNameText);
        DateText = Get<TextMeshProUGUI>((int)Texts.DateText);

        Bind<Slider>(typeof(Sliders));
        MentalSlider = Get<Slider>((int)Sliders.MentalSlider);

        Bind<Button>(typeof(Buttons));
        LiveButton = Get<Button>((int)Buttons.LiveButton);
        BindEvent(LiveButton.gameObject, OnClickedLiveButton, GameEvents.UIEvent.Click);
        PracticeButton = Get<Button>((int)Buttons.PracticeButton);
        BindEvent(PracticeButton.gameObject, OnClickedPracticeButton, GameEvents.UIEvent.Click);
        DatingButton = Get<Button>((int)Buttons.DatingButton);
        BindEvent(DatingButton.gameObject, OnClickedDatingButton, GameEvents.UIEvent.Click);
        RestButton = Get<Button>((int)Buttons.RestButton);
        BindEvent(RestButton.gameObject, OnClickedRestButton, GameEvents.UIEvent.Click);
        GameObject ComebackButton = Get<Button>((int)Buttons.ComebackButton).gameObject;
        BindEvent(ComebackButton, OnClickedComebackButton, GameEvents.UIEvent.Click);
        GameObject SaveButton = Get<Button>((int)Buttons.SaveButton).gameObject;
        BindEvent(SaveButton, OnClickedSaveButton, GameEvents.UIEvent.Click);
        GameObject SettingButton = Get<Button>((int)Buttons.SettingButton).gameObject;
        BindEvent(SettingButton, OnClickedSettingButton, GameEvents.UIEvent.Click);
        GameObject ExitButton = Get<Button>((int)Buttons.ExitButton).gameObject;
        BindEvent(ExitButton, OnClickedExitButton, GameEvents.UIEvent.Click);

        initialized = true;
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
    /// <param name="activeCombackPrepare"> 컴백준비 상태일 경우 true로 하기</param>
    public void Init(string date, string groupName, string fanNum, float mental, string name)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        FanText.text = groupName;
        NameText.text = name;
        GroupNameText.text = groupName;
        DateText.text = fanNum;
        MentalSlider.value = mental;
    }



    /// <summary>
    /// 컴백준비 상태일 때 1회 호출하면 컴백, 연습 외 버튼을 비활성화하는 메서드 
    /// </summary>
    public void CompackPrepareStarted()
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        LiveButton.interactable = false;
        DatingButton.interactable = false;
        RestButton.interactable = false;
    }

    /// <summary>
    /// 한 분기의 마지막 주간에도 컴백을 안 했으면 컴백 버튼만 활성화시키는 메서드  
    /// </summary>
    public void MustComebackStarted()
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        LiveButton.interactable = false;
        DatingButton.interactable = false;
        RestButton.interactable = false;
        PracticeButton.interactable = false;
    }

    /// <summary>
    //메인화면 UI 초기화 메서드 
    /// </summary>
    public void ResetActionButtons()
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        LiveButton.interactable = true;
        DatingButton.interactable = true;
        RestButton.interactable = true;
        PracticeButton.interactable = true;
    }

}
