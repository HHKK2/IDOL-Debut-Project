using System;
using UnityEngine;
using UnityEngine.UI;

public class FanServiceSceneController : MonoBehaviour
{
        public static event Action OnFinished;

    // State에서 주입받는 값
    private static Gender playerGender;

    [Header("UI")]
    [SerializeField] private Image liveImage;

    [Header("Dialogue")]
    [SerializeField] private DialogueSequencePlayer dialoguePlayer;
    [SerializeField] private DialogueText fanServiceDialogue;
    
    [Header("Live Images")]
    [SerializeField] private Sprite[] maleLiveImages;
    [SerializeField] private Sprite[] femaleLiveImages;

    // FanService.Enter()에서 호출
    public static void SetPlayerGender(Gender gender)
    {
        playerGender = gender;
    }

    private void Start()
    {
        SetRandomLiveImage();
        if (dialoguePlayer != null && fanServiceDialogue != null)
        {
            dialoguePlayer.Play(fanServiceDialogue);
        }
    }

    private void SetRandomLiveImage()
    {
        Sprite[] source =
            playerGender == Gender.MALE ? maleLiveImages : femaleLiveImages;

        if (source == null || source.Length == 0)
        {
            Debug.LogError("FanService: 라이브 이미지가 비어 있음");
            return;
        }

        int index = UnityEngine.Random.Range(0, source.Length);
        liveImage.sprite = source[index];
    }

    // Image(Button)의 OnClick에 연결
    public void OnClickLiveImage()
    {
        if (dialoguePlayer != null && dialoguePlayer.IsPlaying)
        {
            return;
        }
        Debug.Log("팬 서비스 씬 종료");
        OnFinished?.Invoke();
    }
}
