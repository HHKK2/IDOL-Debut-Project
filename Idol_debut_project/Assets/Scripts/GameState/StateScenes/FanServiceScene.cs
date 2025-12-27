using System;
using UnityEngine;

public class FanServiceScene : MonoBehaviour
{
    public static event Action OnFinished;

    // 예: “확인/완료” 버튼의 OnClick에 연결
    public void OnClickFinish()
    {
        OnFinished?.Invoke();
    }
}
