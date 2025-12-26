using UnityEngine;

public class Rest : IGameState
{
    private GameStateMachine gsm;
    private Player player;
    private TimeCycleManager time;

    public Rest(GameStateMachine gsm, Player player, TimeCycleManager time)
    {
        this.gsm = gsm;
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

        player.MentalHealth += 10;

        time.AdvanceMonth(); //한 주 지남.

        GameManager.Instance.OnActionStateFinished();
    }
}
