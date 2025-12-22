using UnityEngine;

public class Training : IGameState
{
    Player player;
    TimeCycleManager time;
    GameFlowManager gameFlow;

    public Training(Player player, TimeCycleManager time, GameFlowManager gameFlow)
    {
        this.player = player;
        this.time = time;
        this.gameFlow = gameFlow;
    }

    public void Enter()
    {
        Debug.Log("연습 상태 진입");
    }

    public void Update()
    {
        Debug.Log("연습 상태 로직 처리중");
    }

    public void Exit()
    {
        Debug.Log("연습 상태 종료");
        time.AdvanceWeek(); //한 주 지나감.
        
        gameFlow.OnActionFinished();
    }
}
