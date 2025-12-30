using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// 강의링크: https://www.youtube.com/watch?v=A1kPszkvl44
/// UI 버전으로 변환하여 Overlay Canvas 위에서도 표시 가능
/// </summary>
public class StarEffect : MonoBehaviour
{
    private Image image;
    private RectTransform rectTransform;
    private Vector2 direction;
    public float moveSpeed = 10f;
    public float minSize = 20f;
    public float maxSize = 60f;
    public float sizeSpeed = 1f;
    public Color[] colors;
    public float colorSpeed = 5f;
    
    private Vector2 currentSize;

    private void Start()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        
        direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        float size = Random.Range(minSize, maxSize);
        currentSize = new Vector2(size, size);
        rectTransform.sizeDelta = currentSize;
        
        image.color = colors[Random.Range(0, colors.Length)];
    }

    private void Update()
    {
        rectTransform.anchoredPosition += direction * moveSpeed * Time.deltaTime * 60f;
        
        currentSize = Vector2.Lerp(currentSize, Vector2.zero, Time.deltaTime * sizeSpeed);
        rectTransform.sizeDelta = currentSize;

        Color color = image.color;
        color.a = Mathf.Lerp(image.color.a, 0, Time.deltaTime * colorSpeed);
        image.color = color;

        if (image.color.a <= 0.01f)
        {
            Destroy(gameObject);
        }
    }
}
