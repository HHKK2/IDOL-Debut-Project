using UnityEngine;
using System.Collections;

/// <summary>
/// 임시로 튜토리얼을 넘길 방법이 필요해서 대충 짜놓음... 다 삭제하고 다시 짜는 걸 ㅊㅊ
/// tutorialScene에 있는 버튼도 삭제하십쇼
/// 테스트용으로 튜토리얼 씬에서 버튼을 누르면 homescene으로 넘어가게 해놓음
/// </summary>

public class TutorialSceneController : MonoBehaviour
{
    public void OnClickFinishTutorial()
    {
        GameManager.Instance.StartGame();
        GameSceneManager.Instance.ChangeScene(GameScenes.HomeScene);
    }



}
