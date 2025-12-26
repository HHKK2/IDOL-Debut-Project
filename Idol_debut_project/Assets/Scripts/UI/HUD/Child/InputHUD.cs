using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Data;
using TMPro;

public class InputHUD : UIHUD
{
    
    
    enum GameObjects
    {
        InputInfo,
        InputGender,
        InputGroupName
    }
    
    enum Buttons
    {
        InputInfoNextButton,
        InputGenderNextButton,
        GroupNameNextButton,
        MaleButton,
        FemaleButton
    }

    enum InputFields
    {
        NameInputField,
        GroupNameInputField
    }

    enum Texts
    {
        GenderGroupNoticeText
    }

    public Action<PlayerInfoData> InputActionFinished;

    private GameObject InputInfo;
    private GameObject InputGender; 
    private GameObject InputGroupName;

    private Button InputInfoNextButton;
    private Button InputGenderNextButton;
    private Button GroupNameNextButton;
    private Button MaleButton;
    private Button FemaleButton;
    
    private TMP_InputField  NameInputField;
    private TMP_InputField  GroupNameInputField;

    private TextMeshProUGUI GenderGroupNoticeText;

    private PlayerInfoData playerInfoData;

    private void Start()
    {
        base.Init();
        
        Bind<GameObject>(typeof(GameObjects));
        InputInfo =  Get<GameObject>((int)GameObjects.InputGender);
        InputGender =  Get<GameObject>((int)GameObjects.InputGroupName);
        InputGroupName = Get<GameObject>((int)GameObjects.InputGroupName);
        
        Bind<Button>(typeof(Buttons));
        InputInfoNextButton =   Get<Button>((int)Buttons.InputInfoNextButton);
        BindEvent(InputInfoNextButton.gameObject,OnClickedInputInfoNextButton, GameEvents.UIEvent.Click);
        InputGenderNextButton =   Get<Button>((int)Buttons.InputGenderNextButton);
        BindEvent(InputGenderNextButton.gameObject,OnClickedInputGenderNextButton, GameEvents.UIEvent.Click);
        GroupNameNextButton =   Get<Button>((int)Buttons.GroupNameNextButton);
        BindEvent(GroupNameNextButton.gameObject,OnClickedGroupNameNextButton, GameEvents.UIEvent.Click);
        MaleButton = Get<Button>((int)Buttons.MaleButton);
        BindEvent(MaleButton.gameObject,OnClickedGroupMaleButton, GameEvents.UIEvent.Click);
        FemaleButton = Get<Button>((int)Buttons.FemaleButton);
        BindEvent(FemaleButton.gameObject,OnClickedGroupFemaleButton, GameEvents.UIEvent.Click);
        
        
        
        Bind<TMP_InputField>(typeof(InputFields));
        NameInputField =   Get<TMP_InputField>((int)InputFields.NameInputField);
        NameInputField.onEndEdit.AddListener(OnEndEditNameInputField); 
        GroupNameInputField =    Get<TMP_InputField>((int)InputFields.GroupNameInputField);
        GroupNameInputField.onEndEdit.AddListener(OnEndEditGroupNameInputField);

        Bind<TextMeshProUGUI>(typeof(Texts));
        GenderGroupNoticeText = Get<TextMeshProUGUI>((int)Texts.GenderGroupNoticeText);

    }

    private void OnEndEditNameInputField(string text)
    {
        playerInfoData.name = text;
    }
    private void OnEndEditGroupNameInputField(string text)
    {
        playerInfoData.groupName = text;
    }
    
    private void OnClickedInputInfoNextButton(PointerEventData eventData)
    {
        InputInfo.active = false;
        InputGender.active = true;
        InputGroupName.active = false;
    }
    
    private void OnClickedInputGenderNextButton(PointerEventData eventData)
    {
        InputInfo.active = false;
        InputGender.active = false;
        InputGroupName.active = true;
    }
    
    private void OnClickedGroupNameNextButton(PointerEventData eventData)
    {
        InputActionFinished.Invoke(playerInfoData);
    }
    
    private void OnClickedGroupMaleButton(PointerEventData eventData)
    {
        playerInfoData.gender = 1;
        GenderGroupNoticeText.text = "You have to debut male group.";
    }
    
    private void OnClickedGroupFemaleButton(PointerEventData eventData)
    {
        playerInfoData.gender = 0;   
        GenderGroupNoticeText.text = "You have to debut female group.";
    }
    
}
