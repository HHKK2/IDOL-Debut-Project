using System;
using Data;
using UnityEngine;

public class TwitterHUD : UIHUD
{
    private void Start()
    {
        base.Init();
    }
    

    /// <param name="feeling"></param>
    public void Init(AudianceData.EAudianceFeeling audianceFeeling,  bool isReputationPositiveNumber)
    {
        //TODO: 관객 반응마다 다른 트위터 반응 호출 
    }
    
}
