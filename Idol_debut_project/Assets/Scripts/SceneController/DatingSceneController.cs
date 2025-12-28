using System;
using UnityEngine;
using UnityEngine.UI;

public class DatingSceneController : MonoBehaviour
{
    public static event Action OnFinished;

    private static Gender playerGender;
    private static Dating.DatingResult datingResult;

    [Header("UI")]
    [SerializeField] private Image datingImage;

    [Header("Dispatch Images (1 each)")]
    [SerializeField] private Sprite maleDispatchImage;
    [SerializeField] private Sprite femaleDispatchImage;

    [Header("MentalUp Images")]
    [SerializeField] private Sprite[] maleDatingImages;
    [SerializeField] private Sprite[] femaleDatingImages;

    [Header("Breakup Images")]
    [SerializeField] private Sprite[] maleBreakupImages;
    [SerializeField] private Sprite[] femaleBreakupImages;

    // State에서 호출
    public static void SetContext(Gender gender, Dating.DatingResult result)
    {
        playerGender = gender;
        datingResult = result;
    }

    private void Start()
    {
        SetDatingImage();
    }

    private void SetDatingImage()
    {
        Sprite selected = null;

        switch (datingResult)
        {
            case Dating.DatingResult.Dispatch:
                selected = playerGender == Gender.MALE
                    ? maleDispatchImage
                    : femaleDispatchImage;
                break;

            case Dating.DatingResult.MentalUp:
                selected = GetRandomFrom(
                    playerGender == Gender.MALE ? maleDatingImages : femaleDatingImages
                );
                break;

            case Dating.DatingResult.Breakup:
                selected = GetRandomFrom(
                    playerGender == Gender.MALE ? maleBreakupImages : femaleBreakupImages
                );
                break;
        }

        if (selected == null)
        {
            Debug.LogError("[DatingScene] 선택된 이미지가 없음");
            return;
        }

        datingImage.sprite = selected;
    }

    private Sprite GetRandomFrom(Sprite[] source)
    {
        if (source == null || source.Length == 0)
            return null;

        return source[UnityEngine.Random.Range(0, source.Length)];
    }

    // Image(Button)에 연결
    public void OnClickDatingImage()
    {
        Debug.Log("[DatingScene] Click → Finish");
        OnFinished?.Invoke();
    }
}
