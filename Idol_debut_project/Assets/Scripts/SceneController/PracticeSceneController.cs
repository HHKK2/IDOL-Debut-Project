using UnityEngine;
public class PracticeSceneController : MonoBehaviour
{

    // 테스트용: 스페이스 누르면 컴백 종료
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("연습 씬 종료 → 상태 종료 요청");
            GameManager.Instance.OnActionStateFinished();
        }
    }
}
