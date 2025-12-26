using System;
using TMPro;
using UnityEngine;

public class TimerHUD : UIHUD
{
    enum Texts
    {
        TimerText
    }
    
    private bool initialized = false;
    private TextMeshProUGUI TimerText;

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
        TimerText = Get<TextMeshProUGUI>((int)Texts.TimerText);

        initialized = true;
    }

    /// <summary>
    /// 타이머 초기화 함수: update에서 호출할 것
    /// </summary>
    public void Init(string timerText)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        
        TimerText.text = timerText;
    }
}
