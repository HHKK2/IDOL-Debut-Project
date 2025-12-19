using UnityEngine;

public class ComeBack : IGameState
{
    public void Enter()
    {
        Debug.Log("컴백 상태 진입");
    }

    public void Update()
    {
        Debug.Log("컴백 상태 로직 처리");
    }

    public void Exit()
    {
        Debug.Log("컴백 상태 종료");
    }
}