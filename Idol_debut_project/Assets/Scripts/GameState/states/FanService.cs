using UnityEngine;

public class FanService : IGameState
{
    private GameStateMachine gsm;
    private Player player;
    private TimeCycleManager time;

    public FanService(GameStateMachine gsm, Player player, TimeCycleManager time)
    {
        this.gsm = gsm;
        this.player = player;
        this.time = time;
    }

    public void Enter()
    {
        Debug.Log("팬 서비스 상태 진입");
    }

    public void Update()
    {
        //끝나면 gamestatemachine 쪽에서 exit 호출할 거임
        Debug.Log("팬 서비스 상태 로직 처리");
    }

    public void Exit()
    {
        if (player.Reputation >= 0)
        {
            player.FanNumber += player.Reputation * 10; // 팬 수 증가: 평판 * 10
        }
        else
        {
            player.Reputation += 10;
        }
        player.MentalHealth -= 5; // 멘탈 감소

        time.AdvanceMonth(); // 1주 경과

        GameManager.Instance.OnActionStateFinished();

        Debug.Log("팬 서비스 상태 종료");
    }
}
