using System;
using UnityEngine;

public class FanServiceSceneController : MonoBehaviour
{
    public static event Action OnFinished;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 임시 종료 트리거
        {
            Debug.Log("팬 서비스 씬 종료 → 상태 종료 요청");
            OnFinished?.Invoke();
        }
    }
}
