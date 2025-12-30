using System;
using Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StageHUD : UIHUD
{

    enum Images
    {
        AudianceImage,
    }

    enum Texts
    {
        LyricsText,
        SongTimerText,
        NextLyricsText
    }

    enum Sliders
    {
        SongTimerSlider,
    }

    private Image AudianceImage;

    private TextMeshProUGUI LyricsText;
    private TextMeshProUGUI SongTimerText;
    private TextMeshProUGUI NextLyricsText;
    

    private Slider  SongTimerSlider;
    
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

        UIManager.Instance.ShowSystemUI<StageFadeInSystemUI>();
        
        base.Init();
        
        Bind<Image>(typeof(Images));
        AudianceImage = Get<Image>((int)Images.AudianceImage);
        
        Bind<TextMeshProUGUI>(typeof(Texts));
        LyricsText = Get<TextMeshProUGUI>((int)Texts.LyricsText);
        SongTimerText =  Get<TextMeshProUGUI>((int)Texts.SongTimerText);
        NextLyricsText= Get<TextMeshProUGUI>((int)Texts.NextLyricsText);
        
        Bind<Slider>(typeof(Sliders));
        SongTimerSlider = Get<Slider>((int)Sliders.SongTimerSlider);
        
        initialized = true;
    }
    ///<summary>
    /// !!!!레거시임 민경 코드로 쓰기!!!!
    /// 관객이미지 바꿀 때 호출
    /// </summary>
    public void InitAudianceImage(AudianceData.EAudianceFeeling e)
    { 
        if (!initialized)
        {
            EnsureInitialized();
        }
        switch (e)
        {
            case AudianceData.EAudianceFeeling.Bad:
                AudianceImage.sprite = Resources.Load<Sprite>("Sprites/AudianceImage/Bad");
                break;
            case AudianceData.EAudianceFeeling.Good:
                AudianceImage.sprite = Resources.Load<Sprite>("Sprites/AudianceImage/Good");
                break;
            case  AudianceData.EAudianceFeeling.Huh:
                AudianceImage.sprite = Resources.Load<Sprite>("Sprites/AudianceImage/Huh");
                break;
            case AudianceData.EAudianceFeeling.Perfect:
                AudianceImage.sprite = Resources.Load<Sprite>("Sprites/AudianceImage/Perfect");
                break;
            case AudianceData.EAudianceFeeling.SoSo:
                AudianceImage.sprite = Resources.Load<Sprite>("Sprites/AudianceImage/Soso");
                break;
        }
    }


    /// <summary>
    /// 가사 바꿀 때마다 호출
    /// </summary>
    /// <param name="lyricsText">음악 가사</param>
    public void InitLyricsText(string lyricsText,string nextlyricsText)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        LyricsText.text = lyricsText;
        NextLyricsText.text = nextlyricsText;
    }

    /// <summary>
    /// 현 음악 재생시간을 update에서 호출
    /// </summary>
    /// <param name="sliderValue">현 음악 재생시간을 0-1사이 값으로 변환하여 넣기</param>
    /// <param name="timerText">mm:ss로 현 음악 재생시간을 넣기</param>
    public void InitSongTimerValue(float sliderValue, string timerText)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        SongTimerSlider.value = sliderValue;
        SongTimerText.text = timerText;
    }
    
}
