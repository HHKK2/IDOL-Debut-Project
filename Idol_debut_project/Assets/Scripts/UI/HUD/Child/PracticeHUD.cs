using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PracticeHUD : UIHUD
{
    enum Images
    {
        AlbumImage,
    }

    enum Texts
    {
        SongTitle,
        SongTimer,
        LyricsText,
        NextLyricsText
    }

    enum Buttons
    {
        RightButton,
        LeftButton,
        PracticeButton,
        ExitButton
    }

    enum Sliders
    {
        SongTimerSlider
    }

    public Action onClickedRightSongButton;
    public Action onClickedLeftSongButton;
    public Action onClickedPracticeButton;
    public Action onClickedExitButton;

    private Image AlbumImage;

    private TextMeshProUGUI SongTitle;
    private TextMeshProUGUI SongTimer;
    private TextMeshProUGUI LyricsText;
    private TextMeshProUGUI NextLyricsText;

    private Button RightButton;
    private Button LeftButton;
    private Button PracticeButton;
    private Button ExitButton;

    private Slider SongTimerSlider;

    //selecting/selected 변환
    private GameObject Selecting;
    private GameObject Selected;


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

        Bind<Image>(typeof(Images));
        AlbumImage = Get<Image>((int)Images.AlbumImage);

        Bind<TextMeshProUGUI>(typeof(Texts));
        SongTitle = Get<TextMeshProUGUI>((int)Texts.SongTitle);
        SongTimer = Get<TextMeshProUGUI>((int)Texts.SongTimer);
        LyricsText = Get<TextMeshProUGUI>((int)Texts.LyricsText);
        NextLyricsText = Get<TextMeshProUGUI>((int)Texts.NextLyricsText);
        
        Bind<Button>(typeof(Buttons));
        RightButton = Get<Button>((int)Buttons.RightButton);
        BindEvent(RightButton.gameObject, OnClickRightButton, GameEvents.UIEvent.Click);
        LeftButton = Get<Button>((int)Buttons.LeftButton);
        BindEvent(LeftButton.gameObject, OnClickLeftButton, GameEvents.UIEvent.Click);
        PracticeButton = Get<Button>((int)Buttons.PracticeButton);
        BindEvent(PracticeButton.gameObject, OnClickPracticeButton, GameEvents.UIEvent.Click);
        ExitButton = Get<Button>((int)Buttons.ExitButton);
        BindEvent(ExitButton.gameObject, OnClickExitButton, GameEvents.UIEvent.Click);

        Bind<Slider>(typeof(Sliders));
        SongTimerSlider = Get<Slider>((int)Sliders.SongTimerSlider);

        Selecting = transform.Find("Selecting").gameObject;
        Selected = transform.Find("Selected").gameObject;


        initialized = true;
    }


    /// /// <param name="albumImagePath">Resources 폴더 내의 상대 경로 (확장자 제외)
    /// 예: "Sprites/AlbumCovers/MySong" (Assets/Resources/Sprites/AlbumCovers/MySong.png 일 경우)</param>
    public void Init(string songTitle, string albumImagePath)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }

        AlbumImage.sprite = Resources.Load<Sprite>(albumImagePath);
        SongTitle.text = songTitle;
    }

    public void InitWithSprite(string songTitle, Sprite albumCover)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }

        AlbumImage.sprite = albumCover;
        SongTitle.text = songTitle;
    } 
    /// <summary>
    /// update에서 호출하기
    /// </summary>
    /// <param name="SongMMSS">MM:SS 형태로 음악재생시간 넘겨주기</param>
    public void InitSongMMSS(string SongMMSS)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }

        SongTimer.text = SongMMSS;
    }

    /// <summary>
    /// 가사가 한줄한줄 바뀔 때마다 호출하기
    /// </summary>
    public void InitLyricsText(string currentText,string nextText)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        LyricsText.text = currentText;
        NextLyricsText.text = nextText;
    }

    /// <summary>
    /// update에서 호출하기
    /// </summary>
    ///<param name="value">MM:SS 형태의 값을 float로 변환하여 넘겨주기(max:1)</param>
    public void InitSongSlider(float value)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }

        SongTimerSlider.value = value;
    }

    //selectingmode
    public void SetSelectingMode()
    {
        if (!initialized)
            EnsureInitialized();

        Selecting.SetActive(true);
        Selected.SetActive(false);
    }

    //selectedmode
    public void SetPlayingMode()
    {
        if (!initialized)
            EnsureInitialized();

        Selecting.SetActive(false);
        Selected.SetActive(true);
    }

    //exitbutton 비활성화 함수
    public void SetExitButtonActive(bool active)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }

        ExitButton.gameObject.SetActive(active);
    }

    public TextMeshProUGUI GetLyricsText()
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        return LyricsText;
    }

    public TextMeshProUGUI GetNextLyricsText()
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        return NextLyricsText;
    }



    private void OnClickRightButton(PointerEventData eventData)
    {
        onClickedRightSongButton?.Invoke();
    }
    private void OnClickLeftButton(PointerEventData eventData)
    {
        onClickedLeftSongButton?.Invoke();
    }
    private void OnClickPracticeButton(PointerEventData eventData)
    {
        onClickedPracticeButton?.Invoke();
    }
    private void OnClickExitButton(PointerEventData eventData)
    {
        onClickedExitButton?.Invoke();
    }


}
