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

    private float remainingTime;
    private bool isRunning = false;
    public event Action OnTimerFinished;

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

    private void Update()
    {
        if (!isRunning) return;
        
        remainingTime -= Time.deltaTime;
        
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            isRunning = false;
            UpdateTimerDisplay();
            OnTimerFinished?.Invoke();
            return;
        }
        
        UpdateTimerDisplay();
    }

    public void StartCountdown(float seconds)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }
        remainingTime = seconds;
        isRunning = true;
        UpdateTimerDisplay();
    }

    public void Pause()
    {
        isRunning = false;
    }

    public void Resume()
    {
        if (remainingTime > 0)
        {
            isRunning = true;
        }
    }

    public void Stop()
    {
        isRunning = false;
        remainingTime = 0;
        UpdateTimerDisplay();
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }
    
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        TimerText.text = $"{minutes:00}:{seconds:00}";
    }
}
