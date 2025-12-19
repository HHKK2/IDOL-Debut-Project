using UnityEngine;

public class Rest : IGameState
{
    public void Enter()
    {
        Debug.Log("휴식 상태 진입");
    }

    public void Update()
    {
        Debug.Log("휴식상태 로직 처리중 ");
    }

    public void Exit()
    {
        Debug.Log("휴식 상태 종료 ");
    }
}
