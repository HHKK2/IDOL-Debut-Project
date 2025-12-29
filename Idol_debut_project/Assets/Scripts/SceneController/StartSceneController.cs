using UnityEngine;

public class StartSceneController : MonoBehaviour
{
    private HomeHUD homeHUD;

    void Start()
    {
        // 게임 시작 시 모든 AudioSource 정지 (PersistentScene 포함)
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (AudioSource source in allAudioSources)
        {
            if (source.isPlaying)
            {
                source.Stop();
                Debug.Log($"[StartScene] AudioSource 정지: {source.name}, clip: {(source.clip != null ? source.clip.name : "null")}");
            }
        }

        // HUD 찾기
        homeHUD = UIManager.Instance.HUDList.Find(h => h is HomeHUD) as HomeHUD;

        if (homeHUD == null)
            homeHUD = UIManager.Instance.ShowHUDUI<HomeHUD>();

        // 버튼 이벤트 연결
        homeHUD.ClickedNewGameButton += OnNewGame;
        homeHUD.ClickedLoadButton += OnLoad;
        homeHUD.ClickedExitButton += OnExit;
    }

    private void OnDestroy()
    {
        if (homeHUD == null) return;

        homeHUD.ClickedNewGameButton -= OnNewGame;
        homeHUD.ClickedLoadButton -= OnLoad;
        homeHUD.ClickedExitButton -= OnExit;
    }

    private void OnNewGame()
    {
        GameManager.Instance.ClearLoadedGame();
        // 튜토리얼 시작
        GameSceneManager.Instance.ChangeScene(GameScenes.TutorialScene);
    }

    private void OnLoad()
    {
        bool ok = SaveManager.Instance.LoadGame();
        if (!ok)
        {
            Debug.Log("저장 데이터 없음");
            return;
        }
        
        SaveManager.Instance.Load();
        GameManager.Instance.MarkLoadedGame();
        

        GameSceneManager.Instance.ChangeScene(GameScenes.HomeScene);
    }

    private void OnExit()
    {
        Application.Quit();
    }
}
