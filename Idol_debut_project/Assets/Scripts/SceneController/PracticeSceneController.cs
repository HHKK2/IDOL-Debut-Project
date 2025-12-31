using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class PracticeSceneController : MonoBehaviour
{
    //노래방이랑 한 줄 한 줄 연결.
    [SerializeField] private KaraokeLinePlayer karaokePlayer;


    public static event Action OnFinished;

    private PracticeHUD practiceHUD;
    private PracticeResultHUD resultHUD;

    private ComebackScenarioSO songScenarioSO;
    private Gender playerGender;

    private List<ComebackSongData> unlockedSongs;
    private int currentIndex;

    private enum PracticeState
    {
        Selecting,
        Playing,
        Result
    }

    private PracticeState state;

    private float playTime;
    private float clipLength;
    private AudioSource practiceAudioSource;

    private VocalJudge vocalJudge;
    private VocalResult vocalResult;
    private Coroutine resultDelayCoroutine;
    private bool isResultShown = false;


    private void Start()
    {
        // 데이터 가져오기 (Inspector X)
        songScenarioSO = GameManager.Instance.comebackScenario;
        playerGender = GameManager.Instance.player.Gender;

        // HUD 생성
        practiceHUD = UIManager.Instance.ShowHUDUI<PracticeHUD>();

        // 이벤트 바인딩
        BindHUDEvents();

        //연습씬에서는 그냥 exit button을 꺼둡니다.
        practiceHUD.SetExitButtonActive(false);

        practiceAudioSource = gameObject.AddComponent<AudioSource>();
        practiceAudioSource.loop = false;

        // 곡 리스트 구성
        BuildUnlockedSongList();

        state = PracticeState.Selecting;
        currentIndex = 0;

        RefreshPracticeHUD();
    }

    private void Update()
    {
        if (state == PracticeState.Playing && practiceHUD != null)
        {
            playTime += Time.deltaTime;

            practiceHUD.InitSongMMSS(FormatTime(playTime));
            practiceHUD.InitSongSlider(playTime / clipLength);

            // 곡 끝
            if (playTime >= clipLength)
            {
                EndPractice();
            }
        }
    }

    // ───────────────── 이벤트 바인딩 ─────────────────

    private void BindHUDEvents()
    {
        practiceHUD.onClickedRightSongButton += OnClickRight;
        practiceHUD.onClickedLeftSongButton += OnClickLeft;
        practiceHUD.onClickedPracticeButton += OnClickPractice;
        practiceHUD.onClickedExitButton += OnClickExit;
    }

    // ───────────────── 곡 리스트 ─────────────────

    private void BuildUnlockedSongList()
    {
        unlockedSongs = songScenarioSO.entries
            .Select(e => e.GetSongByGender(playerGender))
            .Where(song => song != null && song.UsedInComeback)
            .OrderByDescending(song =>
                songScenarioSO.entries
                    .First(e => e.GetSongByGender(playerGender) == song)
                    .order)
            .ToList();
    }

    private void RefreshPracticeHUD()
    {
        if (unlockedSongs.Count == 0) return;

        var song = unlockedSongs[currentIndex];
        string albumPath = $"Sprites/AlbumCovers/{song.albumCover.name}";

        practiceHUD.Init(song.title, albumPath);
    }

    // ───────────────── 버튼 처리 ─────────────────

    private void OnClickLeft()
    {
        if (state != PracticeState.Selecting) return;

        currentIndex = Mathf.Max(0, currentIndex - 1);
        RefreshPracticeHUD();
    }

    private void OnClickRight()
    {
        if (state != PracticeState.Selecting) return;

        currentIndex = Mathf.Min(unlockedSongs.Count - 1, currentIndex + 1);
        RefreshPracticeHUD();
    }

    private void OnClickPractice()
    {
        if (unlockedSongs == null || unlockedSongs.Count == 0)
        {
            Debug.LogError("[Practice] unlockedSongs is empty");
            return;
        }

        if (currentIndex < 0 || currentIndex >= unlockedSongs.Count)
        {
            Debug.LogError($"[Practice] invalid index: {currentIndex}");
            return;
        }

        if (state != PracticeState.Selecting)
            return;

        state = PracticeState.Playing;
        playTime = 0f;

        practiceHUD.SetPlayingMode();

        practiceHUD.InitSongMMSS("00:00");
        practiceHUD.InitSongSlider(0f);

        var song = unlockedSongs[currentIndex];
        var clip = unlockedSongs[currentIndex].GetPracticeClip();

        if (clip == null)
        {
            Debug.LogError("[Practice] Practice clip is null");
            return;
        }
        clipLength = clip.length;

        practiceAudioSource.clip = clip;
        practiceAudioSource.Play();

        //노래방 한 줄 한 줄 
        if (karaokePlayer == null)
        {
            Debug.LogError("[Practice] KaraokeLinePlayer is null");
            return;
        }

        TextAsset lyricJson = song.karaokeJsonAsset;
        if (lyricJson == null)
        {
            Debug.LogError("[Practice] karaokeJsonAsset is null");
            return;
        }

        karaokePlayer.lyricsText = practiceHUD.GetLyricsText();
        karaokePlayer.nextLyricsText = practiceHUD.GetNextLyricsText();
        
        karaokePlayer.Init(practiceAudioSource, lyricJson);

        // VocalJudge 관련 초기화
        // PracticeHUD의 자식에서 찾거나, 씬에서 직접 찾기
        if (practiceHUD != null && practiceHUD.gameObject != null)
        {
            vocalJudge = practiceHUD.gameObject.GetComponentInChildren<VocalJudge>(true);
        }
        
        // PracticeHUD에서 못 찾으면 씬에서 직접 찾기
        if (vocalJudge == null)
        {
            vocalJudge = FindFirstObjectByType<VocalJudge>(FindObjectsInactive.Include);
        }
        
        if (vocalJudge != null)
        {
            // 결과 초기화
            vocalJudge.ResetResult();

            // audioSource 연결
            vocalJudge.songAudioSource = practiceAudioSource;
            vocalJudge.useAudioSourceTime = true;
            vocalJudge.autoFinishWhenSongEnds = true;

            // OnFinished 이벤트 구독
            vocalJudge.OnFinished += OnVocalJudgeFinished;

            Debug.Log("[PracticeScene] VocalJudge 초기화 완료");
        }
        else
        {
            Debug.LogWarning("[PracticeScene] VocalJudge를 찾을 수 없습니다. 결과 없이 진행합니다.");
        }

        if (vocalJudge != null && vocalJudge.scoreChart != null)
        {
            vocalJudge.scoreChart.Init(song.scoreChartJsonAsset);
        }

        isResultShown = false;
        vocalResult = null;
    }

    private void OnClickExit()
    {
        EndPractice();
    }

    // ───────────────── 연습 종료 ─────────────────

    private void EndPractice()
    {
        if (state == PracticeState.Result) return;

        state = PracticeState.Result;

        Debug.Log("[PracticeScene] EndPractice 호출됨");

        // 기존 코루틴 정지
        if (resultDelayCoroutine != null)
        {
            StopCoroutine(resultDelayCoroutine);
            resultDelayCoroutine = null;
        }

        // VocalJudge가 있으면 결과를 기다리고, 없으면 바로 지연 후 표시
        if (vocalJudge != null)
        {
            Debug.Log("[PracticeScene] VocalJudge가 있음. 결과를 기다립니다...");
            // VocalJudge의 autoFinishWhenSongEnds가 true이면 자동으로 Finish()가 호출됨
            resultDelayCoroutine = StartCoroutine(ShowResultAfterDelay());
        }
        else
        {
            Debug.Log("[PracticeScene] VocalJudge가 없음. 지연 후 결과 표시");
            resultDelayCoroutine = StartCoroutine(ShowResultAfterDelay());
        }
    }

    private void OnVocalJudgeFinished(VocalResult result)
    {
        Debug.Log($"[PracticeScene] OnVocalJudgeFinished 호출됨! finalScore100={result?.finalScore100}, score={result?.score}");
        vocalResult = result;
        // 결과가 준비되면 바로 표시 (지연 코루틴이 있으면 취소)
        if (resultDelayCoroutine != null)
        {
            StopCoroutine(resultDelayCoroutine);
            resultDelayCoroutine = null;
        }
        ShowResultHUD(result);
    }

    private System.Collections.IEnumerator ShowResultAfterDelay()
    {
        const float delaySeconds = 1.5f;
        yield return new WaitForSeconds(delaySeconds);

        // VocalJudge 결과가 있으면 그것을 사용, 없으면 null로 표시
        Debug.Log($"[PracticeScene] ShowResultAfterDelay 완료. vocalResult={vocalResult?.finalScore100 ?? -1}");
        ShowResultHUD(vocalResult);
        resultDelayCoroutine = null;
    }

    private void ShowResultHUD(VocalResult result)
    {
        // 중복 호출 방지
        if (isResultShown)
        {
            Debug.Log("[PracticeScene] ShowResultHUD 중복 호출 방지");
            return;
        }
        isResultShown = true;

        Debug.Log($"[PracticeScene] ShowResultHUD 호출됨. result={result != null}, finalScore100={result?.finalScore100 ?? -1}");

        // 노래 정지
        if (practiceAudioSource != null && practiceAudioSource.isPlaying)
        {
            practiceAudioSource.Stop();
        }

        UIManager.Instance.CloseHUDUI(nameof(PracticeHUD));
        practiceHUD = null;

        resultHUD = UIManager.Instance.ShowHUDUI<PracticeResultHUD>();

        var song = unlockedSongs[currentIndex];
        string albumPath = $"Sprites/AlbumCovers/{song.albumCover.name}";

        if (result != null)
        {
            // VocalJudge 결과가 있는 경우
            int score = result.finalScore100;
            string rankPath = GetRankSpritePath(score);

            Debug.Log($"[PracticeScene] 결과 표시: score={score}, rank={rankPath}");
            resultHUD.Init(
                rankPath,
                albumPath,
                score.ToString(),
                (currentIndex + 1).ToString(),
                song.title
            );
        }
        else
        {
            // VocalJudge 결과가 없는 경우 (기본값)
            int score = 0;
            string rankPath = GetRankSpritePath(score);

            Debug.Log($"[PracticeScene] 결과 없음. 기본값 표시: score={score}, rank={rankPath}");
            resultHUD.Init(
                rankPath,
                albumPath,
                score.ToString(),
                (currentIndex + 1).ToString(),
                song.title
            );
        }

        resultHUD.OnClickToMainButton += () =>
        {
            UIManager.Instance.CloseHUDUI(nameof(PracticeResultHUD));
            OnFinished?.Invoke();
        };
    }

    // ───────────────── 유틸 ─────────────────
    
    private string FormatTime(float time)
    {
        int m = (int)(time / 60);
        int s = (int)(time % 60);
        return $"{m:00}:{s:00}";
    }

    private string GetRankSpritePath(int score)
    {
        if (score >= 90) return "Sprites/Rank/StageResultS";
        else if (score >= 60) return "Sprites/Rank/StageResultA";
        else if (score >= 40) return "Sprites/Rank/StageResultB";
        else if (score >= 10) return "Sprites/Rank/StageResultC";
        else return "Sprites/Rank/StageResultF";
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (practiceHUD != null)
        {
            practiceHUD.onClickedRightSongButton -= OnClickRight;
            practiceHUD.onClickedLeftSongButton -= OnClickLeft;
            practiceHUD.onClickedPracticeButton -= OnClickPractice;
            practiceHUD.onClickedExitButton -= OnClickExit;
        }

        // 코루틴 정지
        if (resultDelayCoroutine != null)
        {
            StopCoroutine(resultDelayCoroutine);
            resultDelayCoroutine = null;
        }

        // AudioSource 정리
        if (practiceAudioSource != null)
        {
            if (practiceAudioSource.isPlaying)
                practiceAudioSource.Stop();

            UnityEngine.Object.Destroy(practiceAudioSource);
            practiceAudioSource = null;
        }

        // VocalJudge 이벤트 구독 해제
        if (vocalJudge != null)
        {
            vocalJudge.OnFinished -= OnVocalJudgeFinished;
            vocalJudge = null;
        }
    }
}

// using System;
// using UnityEngine;

// public class PracticeSceneController : MonoBehaviour
// {
//     public static event Action OnFinished;

//     private void Update()
//     {
//         // 아무 키나 누르면 컴백씬 종료 테스트
//         if (Input.anyKeyDown)
//         {
//             Debug.Log("[TEST] ComebackScene OnFinished invoked");
//             OnFinished?.Invoke();
//         }
//     }
// }
