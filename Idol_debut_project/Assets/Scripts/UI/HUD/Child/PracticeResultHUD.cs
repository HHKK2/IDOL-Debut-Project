using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PracticeResultHUD : UIHUD
{
    enum Images
    {
        RankImage,
        AlbumImage
    }

    enum Texts
    {
        ScoreText,
        ResultText
    }

    enum Buttons
    {
        ToMainButton
    }

    public Action OnClickToMainButton;
    
    private Image RankImage;
    private Image AlbumImage;
    
    private TextMeshProUGUI  ScoreText;
    private TextMeshProUGUI  ResultText;
    
    private Button ToMainButton;
    
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
        RankImage = Get<Image>((int)Images.RankImage);
        AlbumImage = Get<Image>((int)Images.AlbumImage);
        
        Bind<TextMeshProUGUI>(typeof(Texts));
        ScoreText = Get<TextMeshProUGUI>((int)Texts.ScoreText);
        ResultText = Get<TextMeshProUGUI>((int)Texts.ResultText);
        
        Bind<Button>(typeof(Buttons));
        ToMainButton = Get<Button>((int)Buttons.ToMainButton);
        BindEvent(ToMainButton.gameObject, OnClicked_ToMainButton, GameEvents.UIEvent.Click);

        initialized = true;
    }

    /// <param name="rankImagePath">Resources 폴더 내의 상대 경로 (확장자 제외)
    /// 예: "Sprites/AlbumCovers/MySong" (Assets/Resources/Sprites/AlbumCovers/MySong.png 일 경우)</param>
    /// <param name="albumImagePath">Resources 폴더 내의 상대 경로 (확장자 제외)
    /// 예: "Sprites/AlbumCovers/MySong" (Assets/Resources/Sprites/AlbumCovers/MySong.png 일 경우)</param>
    /// <param name="scoreText">예시: 78</param>
    /// <param name="albumNth">예시: 2</param>
    /// <param name="songName">예시: 바나나알러지원숭이</param>
    public void Init(string rankImagePath, string albumImagePath, string scoreText, string albumNth, string songName)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }

        RankImage.sprite = Resources.Load<Sprite>(rankImagePath);
        AlbumImage.sprite = Resources.Load<Sprite>(albumImagePath);
        ScoreText.text = scoreText;
        ResultText.text = $"{albumNth}번째 앨범 <{songName}> 연습 결과";
    }
    
    public void InitWithSprite(Sprite rankSprite, Sprite albumCover, string scoreText, string albumNth, string songName)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        RankImage.sprite = rankSprite;
        AlbumImage.sprite = albumCover;
        ScoreText.text = scoreText;
        ResultText.text = $"{albumNth}번째 앨범 <{songName}> 연습 결과";
    }

    private void OnClicked_ToMainButton(PointerEventData eventData)
    {
        OnClickToMainButton?.Invoke();
    }
}
