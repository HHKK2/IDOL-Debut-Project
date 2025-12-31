using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHUD : UIBase
{
    public override void Init()
    {
    }
    
    private void OnDestroy()
    {
        if (UIManager.Instance != null && UIManager.Instance.HUDList != null)
        {
            bool removed = UIManager.Instance.HUDList.Remove(this);
            if (removed)
            {
                UIManager.Instance.OnHUDListChanged?.Invoke();
            }
        }
    }
    
    
}