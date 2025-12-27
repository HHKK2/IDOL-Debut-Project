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

        // 씬 전환
        GameSceneManager.Instance.ChangeScene(GameScenes.FanServiceScene);

        // 씬 종료 이벤트 구독
        FanServiceSceneController.OnFinished += FinishFanService;
    }

    public void Update()
    {
        //끝나면 gamestatemachine 쪽에서 exit 호출할 거임
        //Debug.Log("팬 서비스 상태 로직 처리");
    }

    public void Exit()
    {
        // 이벤트 해제(정리)
        FanServiceSceneController.OnFinished -= FinishFanService;

        Debug.Log("팬 서비스 상태 종료");
    }

    // =========================
    // 씬에서 끝났다고 알려올 때 실행되는 실제 종료 로직
    // =========================
    private void FinishFanService()
    {
        if (player.Reputation > 0) //평판이 양수면 팬 수가 증가
        {
            player.FanNumber += player.Reputation * 10; // 팬 수 증가: 평판 * 10
        }
        else //평판이 음수면 명성을 늘려줌
        {
            player.Reputation += 10;
        }

        player.MentalHealth -= 5;   // 멘탈 감소
        
        time.AdvanceMonth();        // 1개월 경과

        // 흐름 복귀 + 엔딩 체크
        GameManager.Instance.OnActionStateFinished();
    }
}
