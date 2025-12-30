using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Data;
using TMPro;
// using System.Diagnostics;

public class InputHUD : UIHUD
{
    enum GameObjects
    {
        Page1,
        Page2
    }

    enum Buttons
    {
        DownButton,
        UpButton,
        SignButton,
        GirlButton,
        BoyButton
    }

    enum InputFields
    {
        NameInputField,
        GroupInputField
    }

    enum Texts
    {
        SignText
    }

    private GameObject Page1;
    private GameObject Page2;

    private TextMeshProUGUI SignText;
    
    private Image GirlButton_Image;
    private Image BoyButton_Image;
    
    private Sprite GirlButton_Active_Sprite;
    private Sprite BoyButton_Active_Sprite;
    private Sprite GirlButton_Deactive_Sprite;
    private Sprite BoyButton_Deactive_Sprite;
    
    private PlayerInfoData playerInfoData;
    public Action<PlayerInfoData> InputActionFinished;

    private bool initialized = false;
    private bool isGenderSelected = false;

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

        //스프라이트 로드
        GirlButton_Active_Sprite = Resources.Load<Sprite>("Sprites/InputInfo/버튼_걸_활성화");
        BoyButton_Active_Sprite = Resources.Load<Sprite>("Sprites/InputInfo/버튼_보이_활성화");
        GirlButton_Deactive_Sprite = Resources.Load<Sprite>("Sprites/InputInfo/버튼_걸_비활성화");
        BoyButton_Deactive_Sprite = Resources.Load<Sprite>("Sprites/InputInfo/버튼_보이_비활성화");
        
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Page1 = Get<GameObject>((int)GameObjects.Page1);
        Page2 = Get<GameObject>((int)GameObjects.Page2);
        
        Bind<Button>(typeof(Buttons));
        Button DownButton = Get<Button>((int)Buttons.DownButton);
        BindEvent(DownButton.gameObject,OnClickedDownButton, GameEvents.UIEvent.Click);
        Button UpButton = Get<Button>((int)Buttons.UpButton);
        BindEvent(UpButton.gameObject,OnClickedUpButton, GameEvents.UIEvent.Click);
        Button SignButton = Get<Button>((int)Buttons.SignButton);
        BindEvent(SignButton.gameObject,OnClickedSignButton, GameEvents.UIEvent.Click);
        Button GirlButton = Get<Button>((int)Buttons.GirlButton);
        GirlButton_Image =  GirlButton.GetComponent<Image>();
        BindEvent(GirlButton.gameObject,OnClickedGirlButton, GameEvents.UIEvent.Click);
        Button BoyButton = Get<Button>((int)Buttons.BoyButton);
        BoyButton_Image =   BoyButton.GetComponent<Image>();
        BindEvent(BoyButton.gameObject,OnClickedBoyButton, GameEvents.UIEvent.Click);
        
        Bind<TMP_InputField>(typeof(InputFields));
        TMP_InputField NameInputField =  Get<TMP_InputField>((int)InputFields.NameInputField);
        NameInputField.onEndEdit.AddListener(OnEndEditNameInputField);
        TMP_InputField GroupInputField =  Get<TMP_InputField>((int)InputFields.GroupInputField);
        GroupInputField.onEndEdit.AddListener(OnEndEditGroupInputField);
        
        Bind<TextMeshProUGUI>(typeof(Texts));
        SignText =  Get<TextMeshProUGUI>((int)Texts.SignText);

        initialized = true;
    }

    private void OnEndEditNameInputField(string text)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        playerInfoData.name = text;
        SignText.text = text;
    }
    
    private void OnEndEditGroupInputField(string text)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        playerInfoData.groupName = text;
    }
    
    private void OnClickedBoyButton(PointerEventData eventData)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        isGenderSelected = true;
        playerInfoData.gender = Gender.MALE;
        BoyButton_Image.sprite = BoyButton_Active_Sprite;
        GirlButton_Image.sprite = GirlButton_Deactive_Sprite;
    }

    private void OnClickedGirlButton(PointerEventData eventData)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        isGenderSelected = true;
        playerInfoData.gender = Gender.FEMALE;
        BoyButton_Image.sprite = BoyButton_Deactive_Sprite;
        GirlButton_Image.sprite = GirlButton_Active_Sprite;
    }


    private bool IsAllFieldsFilled()
    {
        bool hasName = !string.IsNullOrEmpty(playerInfoData.name);
        bool hasGroupName = !string.IsNullOrEmpty(playerInfoData.groupName);
        return hasName && hasGroupName && isGenderSelected;
    }

    public void OnClickedSignButton(PointerEventData eventData)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }

        while (!IsAllFieldsFilled())
        {
            Debug.Log("플레이어 이름, 그룹 명, 걸/보이를 포함한 모든 정보를 입력해야 함.");
            return;
        }
        InputActionFinished?.Invoke(playerInfoData);
    }
    private void OnClickedDownButton(PointerEventData eventData)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        Page1.SetActive(false);
        Page2.SetActive(true);
    }
    private void OnClickedUpButton(PointerEventData eventData)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        Page1.SetActive(true);
        Page2.SetActive(false);
    }
}
