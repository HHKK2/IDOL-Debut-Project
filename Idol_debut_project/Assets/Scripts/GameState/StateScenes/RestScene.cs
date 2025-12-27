using System;
using UnityEngine;

public class RestScene : MonoBehaviour
{
    public static event Action OnFinished;

    // 버튼 OnClick에 연결
    //TODO : 나오는 이미지 개수에 따라 클릭 로직 처리
    public void OnClickFinish()
    {
        OnFinished?.Invoke();
    }
}
