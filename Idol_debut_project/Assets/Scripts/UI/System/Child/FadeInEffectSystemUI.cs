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
    private Action onFadeComplete;

    private void Start()
    {
        if (!initialized)
            EnsureInitialized();
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
        canvasGroup.alpha = 1f;

        initialized = true;
    }

    public void FadeIn(Action onComplete = null)
    {
        FadeIn(defaultDuration, onComplete);
    }

    public void FadeIn(float duration, Action onComplete = null)
    {
        if (!initialized) EnsureInitialized();
        onFadeComplete = onComplete;
        StartCoroutine(FadeInCoroutine(duration));
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / duration));
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        
        onFadeComplete?.Invoke();
        
        UIManager.Instance.CloseSystemUI(GameConstants.UI.SystemName.FadeInEffectSystemUI);
    }
}