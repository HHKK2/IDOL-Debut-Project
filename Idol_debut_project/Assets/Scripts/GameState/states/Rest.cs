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

        // 씬 전환
        GameSceneManager.Instance.ChangeScene(GameScenes.RestScene);

        // 씬 종료 이벤트 구독
        RestScene.OnFinished += FinishRest;
    }

    public void Update()
    {
        Debug.Log("휴식상태 로직 처리중 ");
    }

    public void Exit()
    {
        // 이벤트 정리
        RestScene.OnFinished -= FinishRest;
    }

        // =========================
    // 씬에서 호출되는 종료 지점
    // =========================
    private void FinishRest()
    {
        Debug.Log("휴식 상태 종료");

        player.MentalHealth += 10;
        time.AdvanceMonth();

        GameManager.Instance.OnActionStateFinished();
    }
}
