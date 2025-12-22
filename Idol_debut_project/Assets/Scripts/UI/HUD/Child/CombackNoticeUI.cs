using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class CombackNoticeUI : UIHUD
{
    enum Buttons
    {
        NextButton
    }

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

    private TMP_Text DialogBoxText;

    private bool isDetectedMouseClick = false;
    
    private void Start()
    {
        base.Init();
        
        Bind<Button>(typeof(Buttons));
        NextButton = Get<Button>((int)Buttons.NextButton);
        BindEvent(NextButton.gameObject,OnClickedNextButton, GameEvents.UIEvent.Click);
        
        Bind<Image>(typeof(Images));
        AlbumImage = Get<Image>((int)Images.AlbumImage);
        
        Bind<TMP_Text>(typeof(Texts));
        DialogBoxText =  Get<TMP_Text>((int)Texts.DialogBoxText);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDetectedMouseClick = true;
        }

        if (isDetectedMouseClick)
        {
            OnCombackPrepareStart.Invoke();
            isDetectedMouseClick = false;
        }
    }


    /// <param name="albumImagePath">Resources 폴더 내의 상대 경로 (확장자 제외)
    /// 예: "Sprites/AlbumCovers/MySong" (Assets/Resources/Sprites/AlbumCovers/MySong.png 일 경우)</param>
    public void Init(string albumImagePath, string songName)
    {
        AlbumImage.sprite = Resources.Load<Sprite>(albumImagePath);
        DialogBoxText.text = "This Comback song is "+songName+"."; 
    }
    

    private void OnClickedNextButton(PointerEventData eventData)
    {
        isDetectedMouseClick = true;
    }
}
