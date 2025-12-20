using UnityEngine;

public class FanService : IGameState
{
    public Player player;
    public TimeCycleManager time;

    public FanService(Player player, TimeCycleManager time)
    {
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
        player.FanNumber += player.Reputation * 10; // 팬 수 증가: 평판 * 10
        player.MentalHealth -= 5; // 멘탈 감소

        time.AdvanceWeek(); // 1주 경과

        Debug.Log("팬 서비스 상태 종료");
    }
}
