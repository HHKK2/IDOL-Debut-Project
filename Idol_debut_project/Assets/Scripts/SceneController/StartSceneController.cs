using UnityEngine;

public class StartSceneController : MonoBehaviour
{
    private HomeHUD homeHUD;

    void Start()
    {
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
        // 튜토리얼 시작
        GameSceneManager.Instance.ChangeScene(GameScenes.TutorialScene); 
    }

    private void OnLoad()
    {
        //TODO
        Debug.Log("Load Game (나중에 구현)");
    }

    private void OnExit()
    {
        Application.Quit();
    }
}
