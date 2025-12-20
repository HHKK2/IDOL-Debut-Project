using UnityEngine;

public class Rest : IGameState
{
    public Player player;
    public TimeCycleManager time;
    GameFlowManager gameFlow;

    public Rest(Player player, TimeCycleManager time, GameFlowManager gameFlow)
    {
        this.player = player;
        this.time = time;
        this.gameFlow = gameFlow;
    }

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

        player.MentalHealth += 10;

        time.AdvanceWeek(); //한 주 지남.

        gameFlow.CheckEnding();
    }
}
