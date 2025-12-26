using System;
using UnityEngine;
using System.Reflection;
public class UITunner : MonoBehaviour
{
    private enum UIBaseType
    {
        UIPopup,
        UIHUD, 
        UISystem
    }
    
    [SerializeField] private UIBaseType uiBaseType;
    [SerializeField] private string uiName;
    [SerializeField] private bool isClose;
    
    private void Start()
    {
        if (isClose)
        {
            CloseByName(uiName, uiBaseType);
        }
        else
        {
            ShowByName(uiName, uiBaseType);
        }
    }
    
    private void ShowByName(string typeName, UIBaseType baseType){
        Type uiType = Type.GetType(typeName);
        if(uiType == null){
            Debug.LogError($"타입을 찾을 수 없습니다: {typeName}");
            return;
        }

        string methodName;
        switch (baseType)
        {
            case UIBaseType.UIPopup:
                methodName = "ShowPopupUI";
                break;
            case UIBaseType.UIHUD:
                methodName = "ShowHUDUI";
                break;
            case UIBaseType.UISystem:
                methodName = "ShowSystemUI";
                break;
            default:
                methodName = "ShowHUDUI";
                break;
        }
        
        MethodInfo method = typeof(UIManager).GetMethod(methodName);

        if(method==null){
            Debug.LogError($"메서드를 찾을 없습니다: {methodName}");
            return;
        }

        MethodInfo genericMethod = method.MakeGenericMethod(uiType);
        genericMethod.Invoke(UIManager.Instance, new object[]{uiName});
    }
    
    private void CloseByName(string name, UIBaseType baseType)
    {
        switch (baseType)
        {
            case UIBaseType.UIPopup:
                UIManager.Instance.ClosePopupUI();
                break;
            case UIBaseType.UIHUD:
                UIManager.Instance.CloseHUDUI(name);
                break;
            case UIBaseType.UISystem:
                UIManager.Instance.CloseSystemUI(name);
                break;
        }
    }
}
