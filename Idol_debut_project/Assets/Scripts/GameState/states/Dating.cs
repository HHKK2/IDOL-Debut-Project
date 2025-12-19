using UnityEngine;

public class Dating : IGameState
{
    public void Enter()
    {
        Debug.Log("연애 상태 진입");
    }

    public void Update()
    {
        Debug.Log("연애 상태 로직 처리중");
    }

    public void Exit()
    {
        Debug.Log("연애 상태 종료");
    } 
}
