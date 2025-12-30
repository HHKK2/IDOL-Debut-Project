using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Data;

public class DialogueControllerMulti : MonoBehaviour
{
    [Header("버블 오브젝트")]
    public GameObject leftBubble;
    public GameObject rightBubble;
    public GameObject centerBubble;

    [Header("버블 안의 UI")]
    public Image leftImage;
    public TextMeshProUGUI leftNameText;
    public TextMeshProUGUI leftDialogueText;

    public Image rightImage;
    public TextMeshProUGUI rightNameText;
    public TextMeshProUGUI rightDialogueText;

    public TextMeshProUGUI centerDialogueText;

    [Header("타이핑 속도")]
    public float typeSpeed = 0.03f;

    [Header("플레이어 초상화")] 
    public Sprite femalePlayerSprite;
    public Sprite malePlayerSprite;

    private Coroutine typingRoutine;
    
    
    // ✅ 이름 오버라이드 매개변수 추가 (기본값 null)
    public void ShowDialogue(Speaker speaker, string text, string speakerNameOverride = null)
    {
        // 모두 끄기 (널가드)
        HideAll();

        // 타이핑 중단
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }

        text = DialogueTextFormatter.ResolvePlayerTokens(text);
        
        var player = GameManager.Instance.player;
        bool isPlayer = (speaker == null);

        string displayName;
        Sprite portrait;
        SpeakerPosition pos;

        if (isPlayer)
        {
            displayName = $"{player.Name}\n<size=70%>{player.GroupName}</size>";
            if (player.Gender == Gender.FEMALE)
            {
                portrait = femalePlayerSprite;
            }
            else
            {
                portrait = malePlayerSprite;
            }

            pos = SpeakerPosition.Left;
        }
        else
        {
            displayName = !string.IsNullOrEmpty(speakerNameOverride) ? speakerNameOverride : speaker.speakerName;
            portrait = speaker.characterImage;
            pos = speaker.position;
        }
        

        switch (pos)
        {
            case SpeakerPosition.Left:
                if (leftBubble) leftBubble.SetActive(true);
                if (leftImage)
                {
                    leftImage.sprite = portrait;
                    leftImage.enabled = (portrait != null);
                }
                if (leftNameText) leftNameText.text = displayName;
                if (leftDialogueText) typingRoutine = StartCoroutine(TypeText(leftDialogueText, text));
                break;

            case SpeakerPosition.Right:
                if (rightBubble) rightBubble.SetActive(true);
                if (rightImage)
                {
                    rightImage.sprite = portrait;
                    rightImage.enabled = (portrait != null);
                }
                if (rightNameText) rightNameText.text = displayName;
                if (rightDialogueText) typingRoutine = StartCoroutine(TypeText(rightDialogueText, text));
                break;

            case SpeakerPosition.Center:
                if (centerBubble) centerBubble.SetActive(true);
                if (centerDialogueText) typingRoutine = StartCoroutine(TypeText(centerDialogueText, text));
                break;
        }
    }

    public void HideAll()
    {
        if (leftBubble)  leftBubble.SetActive(false);
        if (rightBubble) rightBubble.SetActive(false);
        if (centerBubble) centerBubble.SetActive(false);
    }

    private IEnumerator TypeText(TextMeshProUGUI target, string text)
    {
        if (!target) yield break;

        target.text = "";
        foreach (char c in text)
        {
            target.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
    
    public void EndDialogueAndGoNext(string nextSceneName)
    {
        HideAll();
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
    
}