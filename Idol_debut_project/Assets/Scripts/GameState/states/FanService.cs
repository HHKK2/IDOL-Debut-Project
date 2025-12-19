using UnityEngine;

public class FanService : IGameState
{
    public void Enter()
    {
        Debug.Log("팬 서비스 상태 진입");
    }

    public void Update()
    {
        Debug.Log("팬 서비스 상태 로직 처리");
    }

    public void Exit()
    {
        Debug.Log("팬 서비스 상태 종료");
    }
}
