using UnityEngine;
using UnityEngine.UI;

public class EndingSceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image endingImage;

    [Header("Happy Ending")]
    [SerializeField] private Sprite[] happyMaleImages;
    [SerializeField] private Sprite[] happyFemaleImages;

    [Header("Normal Ending")]
    [SerializeField] private Sprite[] normalMaleImages;
    [SerializeField] private Sprite[] normalFemaleImages;

    [Header("Bad Ending")]
    [SerializeField] private Sprite[] badMaleImages;
    [SerializeField] private Sprite[] badFemaleImages;

    [Header("Wedding Ending")]
    [SerializeField] private Sprite[] weddingMaleImages;
    [SerializeField] private Sprite[] weddingFemaleImages;

    [Header("Superstar Ending")]
    [SerializeField] private Sprite[] superstarMaleImages;
    [SerializeField] private Sprite[] superstarFemaleImages;

    private void Start()
    {
        ApplyEndingImage();
    }

    private void ApplyEndingImage()
    {
        EndingType ending = GameManager.Instance.End;
        Gender gender = GameManager.Instance.player.Gender;

        Sprite[] candidates = GetCandidates(ending, gender);

        if (candidates == null || candidates.Length == 0)
        {
            Debug.LogError(
                $"[EndingScene] No images for Ending={ending}, Gender={gender}"
            );
            return;
        }

        endingImage.sprite = candidates[Random.Range(0, candidates.Length)];
    }

    private Sprite[] GetCandidates(EndingType ending, Gender gender)
    {
        bool isMale = gender == Gender.MALE;

        switch (ending)
        {
            case EndingType.Happy:
                return isMale ? happyMaleImages : happyFemaleImages;

            case EndingType.Normal:
                return isMale ? normalMaleImages : normalFemaleImages;

            case EndingType.Bad:
                return isMale ? badMaleImages : badFemaleImages;

            case EndingType.Wedding:
                return isMale ? weddingMaleImages : weddingFemaleImages;

            case EndingType.Superstar:
                return isMale ? superstarMaleImages : superstarFemaleImages;

            default:
                return null;
        }
    }
}
