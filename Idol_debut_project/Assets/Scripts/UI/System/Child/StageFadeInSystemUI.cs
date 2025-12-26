using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageFadeInSystemUI : UISystem
{

    enum Texts
    {
        CountDownText
    }
    private TextMeshProUGUI CountDownText;
    
    private CanvasGroup canvasGroup;
    [SerializeField] private float fadeInTime = 0.5f;    
    [SerializeField] private float delayTime = 3f;
    private float delayTimer = 0f;
    private bool hasStartedCoroutine = false;
    
    private void Start()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(Texts));
        CountDownText = Get<TextMeshProUGUI>((int)Texts.CountDownText);
        
        /////
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas를 찾을 수 없습니다!");
            return;
        }
        
        canvasGroup = GameObjectUtils.GetOrAddComponent<CanvasGroup>(canvas.gameObject);
        canvasGroup.alpha = 1f;
    }

    private void Update() {
        if (hasStartedCoroutine)
            return;
            
        if(delayTimer < delayTime)
        {
            delayTimer += Time.deltaTime;
            UpdateCountDownText();
        }
        else
        {
            hasStartedCoroutine = true;
            CountDownText.text = "";
            StartCoroutine(FadeInCoroutine());
        }
    }
    
    private void UpdateCountDownText()
    {
        float remainingTime = delayTime - delayTimer;
        int countDown = Mathf.CeilToInt(remainingTime);
        
        if (countDown > 0 && countDown <= 3)
        {
            CountDownText.text = countDown.ToString();
        }
        else
        {
            CountDownText.text = "";
        }
    }
    
    private IEnumerator FadeInCoroutine()
    {
        yield return StartCoroutine(FadeIn());
        
        UIManager.Instance.CloseSystemUI(GameConstants.UI.SystemName.StageFadeInSystemUI);
    }
    
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        float duration = fadeInTime;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / duration));
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
    }
}
