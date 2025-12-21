using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInSystemUI : UISystem
{
    private CanvasGroup canvasGroup;
    [SerializeField] private float fadeInTime = 0.5f;    
    [SerializeField] private float delayTime = 3f;
    private float delayTimer = 0f;
    private bool hasStartedCoroutine = false;
    
    private void Start()
    {
        base.Init();
        
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
        }
        else
        {
            hasStartedCoroutine = true;
            StartCoroutine(FadeInCoroutine());
        }
    }
    
    private IEnumerator FadeInCoroutine()
    {
        yield return StartCoroutine(FadeIn());
        
        UIManager.Instance.CloseSystemUI(GameConstants.UI.SystemName.FadeInSystemUI);
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
