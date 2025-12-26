using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 전반의 씬 전환을 담당하는 매니저
/// 씬 이름은 반드시 GameScenes에 정의된 값만 사용
/// </summary>

#region 
public enum ActivityType
{
    Practice,       // 연습 (씬 전환)
    Comeback,       // 컴백 (씬 전환)
    Rest,           // 휴식 (UI)
    Dating,         // 연애 (UI)
    FanService    // 컨텐츠 촬영 (UI)
}
#endregion


public class GameSceneManager : AdolpSingleton<GameSceneManager>
{
    //기본 씬 전환 함수
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // =========================
    // 게임 흐름
    // =========================

    //튜토리얼 시작 함수
    public void StartGame()
    {
        ChangeScene(GameScenes.TutorialScene);
    }

    //홈화면(메인화면)으로 돌아오는 함수
    public void ReturnToHome()
    {
        ChangeScene(GameScenes.HomeScene);
    }

    // =========================
    // 행동 선택 처리
    // =========================

    public void StartActivity(ActivityType activity)
    {
        switch (activity)
        {
            //연습하기 선택
            case ActivityType.Practice:
                ChangeScene(GameScenes.PracticeScene);
                break;

            //컴백하기 선택
            case ActivityType.Comeback:
                ChangeScene(GameScenes.ComebackScene);
                break;

            case ActivityType.Rest:
            case ActivityType.Dating:
            case ActivityType.FanService:
                // 씬 전환 없음, 화면 띄우는 걸로 처리.
                Debug.Log($"{activity} : UI 처리");
                break;
        }
    }

}
