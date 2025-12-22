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
        SongTimer
    }

    enum Buttons
    {
        RightButton,
        LeftButton,
        PracticeButton
    }

    public Action onClickedRightSongButton;
    public Action onClickedLeftSongButton;
    public Action onClickedPracticeButton;
    
    private Image AlbumImage;

    private TextMeshProUGUI SongTitle;
    private TextMeshProUGUI SongTimer;
    
    private Button RightButton;
    private Button LeftButton;
    private Button PracticeButton;

    private void Start()
    {
        base.Init();
        
        Bind<Image>(typeof(Images));
        AlbumImage = Get<Image>((int)Images.AlbumImage);
        
        Bind<TextMeshProUGUI>(typeof(Texts));
        SongTitle = Get<TextMeshProUGUI>((int)Texts.SongTitle);
        SongTimer =  Get<TextMeshProUGUI>((int)Texts.SongTimer);
        
        Bind<Button>(typeof(Buttons));
        RightButton = Get<Button>((int)Buttons.RightButton);
        BindEvent(RightButton.gameObject, OnClickRightButton, GameEvents.UIEvent.Click);
        LeftButton = Get<Button>((int)Buttons.LeftButton);
        BindEvent(LeftButton.gameObject, OnClickLeftButton,GameEvents.UIEvent.Click);
        PracticeButton =  Get<Button>((int)Buttons.PracticeButton);
        BindEvent(PracticeButton.gameObject, OnClickPracticeButton, GameEvents.UIEvent.Click);
    }

    /// <param name="SongMMSS">MM:SS 형태로 음악재생시간 넘겨주기</param>
    /// /// <param name="albumImagePath">Resources 폴더 내의 상대 경로 (확장자 제외)
    /// 예: "Sprites/AlbumCovers/MySong" (Assets/Resources/Sprites/AlbumCovers/MySong.png 일 경우)</param>
    public void Init(string songTitle, string SongMMSS,string albumImagePath )
    {
        AlbumImage.sprite = Resources.Load<Sprite>(albumImagePath);
        SongTitle.text = songTitle;
        SongTimer.text = SongMMSS;
    }


    private void OnClickRightButton(PointerEventData eventData)
    {
        onClickedRightSongButton.Invoke();
    }
    private void OnClickLeftButton(PointerEventData eventData)
    {
        onClickedLeftSongButton.Invoke();
    }
    private void OnClickPracticeButton(PointerEventData eventData)
    {
        onClickedPracticeButton.Invoke();
    }
    
}
