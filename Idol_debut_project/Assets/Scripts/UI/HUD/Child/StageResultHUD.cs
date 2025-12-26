using System;
using Data;
using TMPro;
using UnityEngine;

public class StageResultHUD : UIHUD
{
    enum Texts
    {
        TotalScoreText,
        BakJaText,
        UmJungText
    }

    private bool initialized = false;
    private TextMeshProUGUI TotalScoreText;
    private TextMeshProUGUI BakJaText;
    private TextMeshProUGUI UmJungText;

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
        TotalScoreText =  Get<TextMeshProUGUI>((int)Texts.TotalScoreText);
        BakJaText = Get<TextMeshProUGUI>((int)Texts.BakJaText);
        UmJungText =  Get<TextMeshProUGUI>((int)Texts.UmJungText);

        initialized = true;
    }

    public void Init(string totalScoreText, string bakJaText, string umJungText, AudianceData.EAudianceFeeling audianceFeeling, bool isReputationPositiveNumber)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        TotalScoreText.text = totalScoreText;
        BakJaText.text = bakJaText;
        UmJungText.text = umJungText;

        TwitterHUD twitterHUD =  UIManager.Instance.ShowHUDUI<TwitterHUD>();
        twitterHUD.Init(audianceFeeling,isReputationPositiveNumber);
    }
}
