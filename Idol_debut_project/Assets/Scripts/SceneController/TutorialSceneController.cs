using UnityEngine;
using System.Collections;

/// <summary>
/// 임시로 튜토리얼을 넘길 방법이 필요해서 대충 짜놓음...
/// tutorialScene에 있는 남/여 버튼도 삭제하십쇼
/// 테스트용으로 튜토리얼 씬에서 남자/여자를 선택하면 homescene으로 넘어가게 해놓음
/// </summary>

public class TutorialSceneController : MonoBehaviour
{
    public void OnClickSelectMale()
    {
        SetGenderAndStart(Gender.MALE);
    }

    public void OnClickSelectFemale()
    {
        SetGenderAndStart(Gender.FEMALE);
    }

    private void SetGenderAndStart(Gender gender)
    {
        // 🔹 플레이어 성별 세팅 (임시)
        GameManager.Instance.player.Gender = gender;

        Debug.Log($"[Tutorial] Gender selected: {gender}");

        // 🔹 게임 시작
        GameManager.Instance.StartGame();

        // 🔹 홈 씬으로 이동
        GameSceneManager.Instance.ChangeScene(GameScenes.HomeScene);
    }
}
