using UnityEngine;

public class Rest : IGameState
{
    public Player player;
    public TimeCycleManager time;

    public Rest(Player player, TimeCycleManager time)
    {
        this.player = player;
        this.time = time;
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
        
        time.AdvanceWeek(); //한 주 지남.
    }
}
