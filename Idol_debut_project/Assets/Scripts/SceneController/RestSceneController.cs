using System;
using UnityEngine;
using UnityEngine.UI;

public class RestSceneController : MonoBehaviour
{
    public static event Action OnFinished;

    // State에서 주입받는 값
    private static Gender playerGender;

    [Header("UI")]
    [SerializeField] private Image restImage;

    [Header("Rest Images")]
    [SerializeField] private Sprite[] maleRestImages;
    [SerializeField] private Sprite[] femaleRestImages;

    // Rest.Enter()에서 호출
    public static void SetPlayerGender(Gender gender)
    {
        playerGender = gender;
    }

    private void Start()
    {
        SetRandomRestImage();
    }

    private void SetRandomRestImage()
    {
        Sprite[] source =
            playerGender == Gender.MALE ? maleRestImages : femaleRestImages;

        if (source == null || source.Length == 0)
        {
            Debug.LogError("RestScene: 휴식 이미지가 비어 있음");
            return;
        }

        int index = UnityEngine.Random.Range(0, source.Length);
        restImage.sprite = source[index];
    }

    // Image(Button)에 연결
    public void OnClickRestImage()
    {
        Debug.Log("휴식 씬 종료");
        OnFinished?.Invoke();
    }
}
