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
        SongTimerText
    }

    enum Sliders
    {
        SongTimerSlider,
    }

    private Image AudianceImage;

    private TextMeshProUGUI LyricsText;
    private TextMeshProUGUI SongTimerText;
    

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
        
        Bind<Slider>(typeof(Sliders));
        SongTimerSlider = Get<Slider>((int)Sliders.SongTimerSlider);

        initialized = true;
    }
    ///<summary>
    /// 관객이미지 바꿀 때 호출
    /// </summary>
    public void InitAudianceImage(AudianceData.EAudianceFeeling e)
    { 
        if (!initialized)
        {
            EnsureInitialized();
        }
        //TODO: enum받아서, 다른 경로로 AudianceImage초기화
        //AudianceImage.sprite = Resources.Load<Sprite>(audianceImagePath);
    }


    /// <summary>
    /// 가사 바꿀 때마다 호출
    /// </summary>
    /// <param name="lyricsText">음악 가사</param>
    public void InitLyricsText(string lyricsText)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        LyricsText.text = lyricsText;
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
