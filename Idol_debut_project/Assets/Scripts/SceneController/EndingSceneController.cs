using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingSceneController : MonoBehaviour
{
    [Header("Refs")] 
    public DialogueSequencePlayer SequencePlayer;
    public DialogueBackgroundController bg;

    [Header("Dialogues")] 
    public DialogueText happyDialogue;
    public DialogueText normalDialouge;
    public DialogueText badDialogue;
    public DialogueText hiddenDialouge;
    public DialogueText superDialogue;

    public string defaultBgKey = "intro_image";
    
    
    [Header("UI")]
    [SerializeField] private Image endingImage;
    [SerializeField] private TextMeshProUGUI narrationText;
    [SerializeField] private Image fadePanel;


    [Header("Typing")]
    [SerializeField] private float normalTypingSpeed = 0.07f;
    [SerializeField] private float badTypingSpeed = 0.05f;

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
        // 1. 엔딩 이미지 미리 세팅 + 켜기
        ApplyEndingImage();
        endingImage.gameObject.SetActive(true);

        // 2. 페이드 패널: 검정 화면으로 시작
        fadePanel.gameObject.SetActive(true);
        Color fc = fadePanel.color;
        fc.a = 1f;
        fadePanel.color = fc;

        // 3. 대사 초기화
        narrationText.text = "";
        Color nc = narrationText.color;
        nc.a = 1f;
        narrationText.color = nc;

        StartCoroutine(PlayEndingSequence());
    }


    // =========================
    // 🎬 엔딩 전체 연출 흐름
    // =========================
    private IEnumerator PlayEndingSequence()
    {
        EndingType ending = GameManager.Instance.End;

        // 1. 대사 출력 (타이핑)
        string[] lines = GetEndingLines(ending);
        float typingSpeed = GetTypingSpeed(ending);

        foreach (string line in lines)
        {
            yield return StartCoroutine(TypeLine(line, typingSpeed));
            yield return new WaitForSeconds(1.0f);
        }

        // 2. 페이드 인 (유틸 사용)
        yield return StartCoroutine(FadeEffect.Fade(fadePanel, 1f, 0f, 1.7f));
        
        //3. 대사는 지우기
        narrationText.gameObject.SetActive(false);

        var dialogue = GetEndingDialogue(ending);
        if (SequencePlayer == null)
        {
            Debug.LogError("[EndingScene] SequencePlayer가 연결 안됨");
            yield break;
        }

        if (dialogue == null)
        {
            Debug.LogWarning($"[EndingScene] Ending dialogue가 비어있음: {ending}");
            yield break;
        }
        SequencePlayer.Play(dialogue, () =>
        {
            Debug.Log("[EndingScene] 엔딩 대사 끝!");
        });
    }

    // =========================
    // ⌨ 타이핑 효과
    // =========================
    private IEnumerator TypeLine(string line, float speed)
    {
        narrationText.text = "";

        foreach (char c in line)
        {
            narrationText.text += c;
            yield return new WaitForSeconds(speed);
        }
    }

    // =========================
    // 엔딩별 대사
    // =========================
    private string[] GetEndingLines(EndingType ending)
    {
        switch (ending)
        {
            case EndingType.Normal:
            case EndingType.Happy:
                return new[]
                {
                    " 당신은 3년차 아이돌이 되어 계약이 끝났습니다.",
                    " 그리고..."
                };

            case EndingType.Bad:
                return new[]
                {
                    " 너무 많은 사건들이 있었습니다.",
                    " 당신은 지쳤습니다."
                };

            case EndingType.Wedding:
                return new[]
                {
                    " 디스패치에게 3번 발각되었습니다.",
                    " 이제는 입막음이 어렵습니다.",
                    " 결국 당신은..."
                };

            case EndingType.Superstar:
                return new[]
                {
                    " 당신은 슈퍼스타가 되었습니다!",
                };

            default:
                return new string[0];
        }
    }

    // =========================
    // ⏱ 엔딩별 타이핑 속도
    // =========================
    private float GetTypingSpeed(EndingType ending)
    {
        switch (ending)
        {
            case EndingType.Bad:
                return badTypingSpeed;

            default:
                return normalTypingSpeed;
        }
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


    private DialogueText GetEndingDialogue(EndingType ending)
    {
        switch (ending)
        {
            case EndingType.Happy: return happyDialogue;
            case EndingType.Superstar: return superDialogue;
            case EndingType.Bad: return badDialogue;
            case EndingType.Normal: return normalDialouge;
            case EndingType.Wedding: return hiddenDialouge;
            default: return null;
        }
    }
}
