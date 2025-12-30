using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInEffectSystemUI : UISystem
{
    enum Images
    {
        FadeImage
    }

    private Image FadeImage;
    private CanvasGroup canvasGroup;
    
    [SerializeField] private float defaultDuration = 1.5f;

    private bool initialized = false;
    private bool isDestroyed = false;
    private Action onFadeComplete;

    private void Start()
    {
        if (!initialized)
            EnsureInitialized();
    }

    private void OnDestroy()
    {
        isDestroyed = true;
    }

    private void EnsureInitialized()
    {
        if (initialized) return;

        base.Init();
        
        Bind<Image>(typeof(Images));
        FadeImage = Get<Image>((int)Images.FadeImage);

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas를 찾을 수 없습니다!");
            return;
        }
        
        canvasGroup = GameObjectUtils.GetOrAddComponent<CanvasGroup>(canvas.gameObject);

        initialized = true;
    }

    // ==================== Fade In (검은 화면 → 밝아짐) ====================
    
    public void FadeIn(Action onComplete = null)
    {
        FadeIn(defaultDuration, onComplete);
    }

    public void FadeIn(float duration, Action onComplete = null)
    {
        if (!initialized) EnsureInitialized();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        onFadeComplete = onComplete;
        StartCoroutine(FadeInCoroutine(duration));
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            if (isDestroyed) yield break;
            
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / duration));
            yield return null;
        }
        
        if (isDestroyed) yield break;
        
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        
        onFadeComplete?.Invoke();
    }

    // ==================== Fade Out (밝은 화면 → 검게 됨) ====================
    
    public void FadeOut(Action onComplete = null)
    {
        FadeOut(defaultDuration, onComplete);
    }

    public void FadeOut(float duration, Action onComplete = null)
    {
        if (!initialized) EnsureInitialized();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        gameObject.SetActive(true);
        onFadeComplete = onComplete;
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            if (isDestroyed) yield break;
            
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        
        if (isDestroyed) yield break;
        
        canvasGroup.alpha = 1f;
        
        onFadeComplete?.Invoke();
    }
}