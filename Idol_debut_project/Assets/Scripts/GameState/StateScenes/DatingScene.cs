using System;
using UnityEngine;
using UnityEngine.UI;

public class DatingScene : MonoBehaviour
{
    public static event Action OnFinished;

    [SerializeField] private Image storyImage;
    [SerializeField] private Sprite[] storySprites;

    private int currentIndex = 0;

    void Start()
    {
        currentIndex = 0;
        storyImage.sprite = storySprites[currentIndex];
    }

    // 화면 클릭 or 버튼 클릭
    public void OnClickNext()
    {
        currentIndex++;

        if (currentIndex >= storySprites.Length)
        {
            // 모든 이미지 다 봄 → 종료 신호
            OnFinished?.Invoke();
            return;
        }

        storyImage.sprite = storySprites[currentIndex];
    }
}
