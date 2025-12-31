using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 강의링크: https://www.youtube.com/watch?v=A1kPszkvl44
/// UI 버전: 별 이펙트를 Canvas 위에 생성하여 모든 UI 위에 표시
/// </summary>
public class MouseEffect : AdolpSingleton<MouseEffect>
{
    [SerializeField] private AudioSource audioSource;
    public GameObject starEffectPrefab;
    public int canvasSortOrder = 9999;
    private float spawnsTime = 0f;
    public float defaultTime = 0.05f;
    
    private Canvas starCanvas;
    private RectTransform canvasRectTransform;

    private void Start()
    {
        if (starEffectPrefab == null)
        {
            starEffectPrefab = Resources.Load<GameObject>("Effect/UI_StarEffectPrefab");
        }
        CreateStarCanvas();
    }

    void CreateStarCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas_StarEffect");
        canvasObj.transform.SetParent(transform);
        
        starCanvas = canvasObj.AddComponent<Canvas>();
        starCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        starCanvas.sortingOrder = canvasSortOrder;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // GraphicRaycaster 제거 - 별 이펙트가 클릭을 방해하지 않도록
        
        canvasRectTransform = starCanvas.GetComponent<RectTransform>();
    }
    
    private void Update()
    {
        if (Input.GetMouseButton(0) && spawnsTime >= defaultTime)
        {
            StarCreate();
            spawnsTime = 0f;
        }

        if (Input.GetMouseButtonDown(0))
        {
            SoundManager.Instance.SFXPlay("ClickSound",audioSource.clip);
        }
        spawnsTime += Time.deltaTime;
    }

    void StarCreate()
    {
        if (starCanvas == null) return;
        
        GameObject star = Instantiate(starEffectPrefab, starCanvas.transform);
        RectTransform rectTransform = star.GetComponent<RectTransform>();
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            Input.mousePosition,
            null,
            out Vector2 localPoint
        );
        
        rectTransform.anchoredPosition = localPoint;
    }
}
