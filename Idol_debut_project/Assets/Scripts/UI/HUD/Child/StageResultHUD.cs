using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageResultHUD : UIHUD
{
    enum Images
    {
        RankImage,
    }

    enum Texts
    {
        ScoreText,
        ResultText
    }

    enum Buttons
    {
        GoHomeButton
    }

    public Action OnClickGoHomeButton;
    
    private Image RankImage;
    
    private TextMeshProUGUI  ScoreText;
    private TextMeshProUGUI  ResultText;
    
    private Button GoHomeButton;
    
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
        
        Bind<TextMeshProUGUI>(typeof(Texts));
        ScoreText = Get<TextMeshProUGUI>((int)Texts.ScoreText);
        ResultText = Get<TextMeshProUGUI>((int)Texts.ResultText);
        
        Bind<Button>(typeof(Buttons));
        GoHomeButton = Get<Button>((int)Buttons.GoHomeButton);
        BindEvent(GoHomeButton.gameObject, OnClicked_GoHomeButton, GameEvents.UIEvent.Click);

    
        
        initialized = true;
    }

    /// <param name="rankImagePath">Resources 폴더 내의 상대 경로 (확장자 제외). Assets/Resources/Sprites/Rank에 있습니다.
    /// 예: "Sprites/AlbumCovers/MySong" (Assets/Resources/Sprites/AlbumCovers/MySong.png 일 경우)</param>
    /// <param name="scoreText">예시: 78</param>
    /// <param name="albumNth">예시: 2</param>
    /// <param name="songName">예시: 바나나알러지원숭이</param>
    /// <param name="audianceFeeling">최종 관객 기분 넣기(해당 인자에 따라 트위터 반응 갈리는 내부로직 구현되어있음)</param>
    /// <param name="isReputationPositiveNumber">평판이 양수면 true</param>
    public void Init(string rankImagePath, string scoreText, string albumNth, string songName, AudianceData.EAudianceFeeling audianceFeeling, bool isReputationPositiveNumber)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }

        if (rankImagePath != null)
        {
            RankImage.sprite = Resources.Load<Sprite>(rankImagePath);
        }
        ScoreText.text = scoreText;
        ResultText.text = $"{albumNth}번째 앨범 <{songName}> 연습 결과";
        
        TwitterHUD twitterHUD =  UIManager.Instance.ShowHUDUI<TwitterHUD>();
        twitterHUD.Init(audianceFeeling,isReputationPositiveNumber);
    }

    private void OnClicked_GoHomeButton(PointerEventData eventData)
    {
        OnClickGoHomeButton?.Invoke();
    }
}
