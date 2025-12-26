using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TwitterSlot : UISlot
{
    enum Texts
    {
        NicknameText,
        EmailText,
        MainText
    }
    


    enum Images
    {
        Profile_Img
    }

    private TextMeshProUGUI NicknameText;
    private TextMeshProUGUI EmailText;
    private TextMeshProUGUI MainText;

    private Image Profile_Img;
    
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
        NicknameText =  Get<TextMeshProUGUI>((int)Texts.NicknameText);
        EmailText =  Get<TextMeshProUGUI>((int)Texts.EmailText);
        MainText =  Get<TextMeshProUGUI>((int)Texts.MainText);
        
        
        Bind<Image>(typeof(Images));
        Profile_Img = Get<Image>((int)Images.Profile_Img);
        
        initialized = true;
    }

    /// <param name="profileImagePath">Resources 폴더 내의 상대 경로 (확장자 제외)
    /// 예: "Sprites/AlbumCovers/MySong" (Assets/Resources/Sprites/AlbumCovers/MySong.png 일 경우)</param>
    public void Init(string nickname, string email, string profileImagePath, string maintext)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
       NicknameText.text = nickname; 
       EmailText.text = email;
       MainText.text = maintext;
       if (string.IsNullOrEmpty(profileImagePath))
       {
           Debug.LogError("[TwitterSlot] profileImagePath is null or empty");
           return;
       }
       Profile_Img.sprite = Resources.Load<Sprite>(profileImagePath);
    }
}
