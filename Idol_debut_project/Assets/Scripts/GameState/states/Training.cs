using UnityEngine;

public class Training : IGameState
{
    private GameStateMachine gsm;
    private Player player;
    private TimeCycleManager time;

    public Training(GameStateMachine gsm, Player player, TimeCycleManager time)
    {
        this.gsm = gsm;
        this.player = player;
        this.time = time;
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
        time.AdvanceMonth(); //한 주 지나감.

        GameManager.Instance.OnActionStateFinished();
    }
}
