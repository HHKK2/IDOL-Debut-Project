using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class PracticeSceneController : MonoBehaviour
{
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
        if (state == PracticeState.Playing)
        {
            playTime += Time.deltaTime;

            practiceHUD.InitSongMMSS(FormatTime(playTime));
            practiceHUD.InitSongSlider(playTime / clipLength);

            // 가사 예시 (실제론 타임라인/데이터로 교체)
            UpdateLyrics(playTime);

            // 곡 끝
            if (playTime >= clipLength)
            {
                EndPractice();
            }
        }

        // 임시 종료
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndPractice();
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

        var clip = unlockedSongs[currentIndex].GetPracticeClip();

        if (clip == null)
        {
            Debug.LogError("[Practice] Practice clip is null");
            return;
        }
        clipLength = clip.length;

        practiceAudioSource.clip = clip;
        practiceAudioSource.Play();

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

        if (practiceAudioSource.isPlaying)
        {
            practiceAudioSource.Stop();
        }

        ShowResultHUD();
    }

    private void ShowResultHUD()
    {
        UIManager.Instance.CloseHUDUI(nameof(PracticeHUD));

        resultHUD = UIManager.Instance.ShowHUDUI<PracticeResultHUD>();

        var song = unlockedSongs[currentIndex];

        // 임시 결과값
        int score = UnityEngine.Random.Range(60, 100);
        string rankPath = GetRankSpritePath(score);
        string albumPath = $"Sprites/AlbumCovers/{song.albumCover.name}";

        resultHUD.Init(
            rankPath,
            albumPath,
            score.ToString(),
            (currentIndex + 1).ToString(),
            song.title
        );

        resultHUD.OnClickToMainButton += () =>
        {
            UIManager.Instance.CloseHUDUI(nameof(PracticeResultHUD));
            OnFinished?.Invoke();
        };
    }

    // ───────────────── 가사 / 유틸 ─────────────────

    private void UpdateLyrics(float time)
    {
        //TODO : 민경이의 가사 한 줄 한 줄 띄우는 로직...
    }

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
