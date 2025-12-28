using UnityEngine;
using UnityEngine.UI;

public class VocalUIController : MonoBehaviour
{
    [SerializeField] private VocalJudge vocalJudge;

    [Header("UI")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text detailText;

    void Awake()
    {
        if (vocalJudge != null)
            vocalJudge.OnFinished += OnVocalFinished;
    }

    void OnDestroy()
    {
        if (vocalJudge != null)
            vocalJudge.OnFinished -= OnVocalFinished;
    }

    // 버튼에서 호출할 함수들 ----------------

    public void StartSong()
    {
        // 곡 시작 전에 초기화 + 오디오 재생
        vocalJudge.ResetResult();

        if (vocalJudge.songAudioSource != null)
        {
            vocalJudge.songAudioSource.time = 0f;
            vocalJudge.songAudioSource.Play();
        }
    }

    public void ForceFinish()
    {
        vocalJudge.Finish();
    }

    public void ResetJudge()
    {
        vocalJudge.ResetResult();
        if (scoreText != null) scoreText.text = "Score: 0";
        if (detailText != null) detailText.text = "";
    }

    // 결과 이벤트 받기 ----------------------

    private void OnVocalFinished(VocalResult r)
    {
        // r에 네가 넣은 normalizedScore 같은 게 있으면 그걸 출력하면 됨
        if (scoreText != null)
            scoreText.text = "Score: " + r.score;

        if (detailText != null)
        {
            detailText.text =
                "Perfect: " + r.perfect +
                "\nGood: " + r.good +
                "\nBad: " + r.bad +
                "\nMiss: " + r.miss +
                "\nSilentPenalty: " + r.silentPenalty +
                "\nTotal: " + r.totalCount;
        }
    }
}