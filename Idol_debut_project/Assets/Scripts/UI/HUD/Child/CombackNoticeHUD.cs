using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class CombackNoticeHUD : UIHUD
{


    enum Images
    {
        AlbumImage,
    }

    enum Texts
    {
        DialogBoxText,
    }

    public Action OnCombackPrepareStart;

    private Button NextButton;

    private Image AlbumImage;

    private TextMeshProUGUI DialogBoxText;

    private bool isDetectedMouseClick = false;
    private bool initialized = false;
    private bool hasInvoked = false;
    
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
        DialogBoxText =  Get<TextMeshProUGUI>((int)Texts.DialogBoxText);

        initialized = true;
    }

    private void Update()
    {
        if (hasInvoked)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            isDetectedMouseClick = true;
        }

        if (isDetectedMouseClick)
        {
            hasInvoked = true;
            OnCombackPrepareStart?.Invoke();
            isDetectedMouseClick = false;
        }
    }


    /// <param name="albumImagePath">Resources 폴더 내의 상대 경로 (확장자 제외)
    /// 예: "Sprites/AlbumCovers/MySong" (Assets/Resources/Sprites/AlbumCovers/MySong.png 일 경우)</param>
    public void Init(string albumImagePath,string conceptName ,string songName)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        AlbumImage.sprite = Resources.Load<Sprite>(albumImagePath);
        DialogBoxText.text = "이번 컴백 곡은 "+conceptName+" 컨셉의 '"+songName+"'이다."; 
    }
    
}
