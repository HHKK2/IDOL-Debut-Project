using UnityEngine;

public class FanService : IGameState
{
    private GameStateMachine gsm;
    private Player player;


    public FanService(GameStateMachine gsm, Player player)
    {
        this.gsm = gsm;
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log("팬 서비스 상태 진입");

        // 씬 전환
        GameSceneManager.Instance.ChangeScene(GameScenes.FanServiceScene);

        // 씬 종료 이벤트 구독
        FanServiceSceneController.OnFinished += FinishFanService;

        //플레이어 성별 전달(이미지가 달라짐)
        FanServiceSceneController.SetPlayerGender(player.Gender);
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
        // =========================
        // 1. 팬 수 / 평판 처리
        // =========================
        if (player.Reputation > 0)
        {
            // 팬 수: 0 ~ 무한
            int fanGain = player.Reputation * 50;
            player.FanNumber = Mathf.Max(0, player.FanNumber + fanGain);
        }
        else
        {
            // 평판: -100 ~ 100
            player.Reputation = Mathf.Clamp(player.Reputation + 10, -100, 100);
        }

        // =========================
        // 2. 멘탈 감소 (0 ~ 100)
        // =========================
        player.MentalHealth = Mathf.Clamp(
            player.MentalHealth - 5,
            0,
            100
        );

        // =========================
        // 3. 엔딩 체크 + 복귀
        // =========================
        GameManager.Instance.CheckImmediateEnding();
        if (GameManager.Instance.isGameEnded)
            return;

        GameManager.Instance.OnActionStateFinished();
    }

}
