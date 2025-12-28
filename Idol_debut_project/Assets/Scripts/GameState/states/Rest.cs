using UnityEngine;

public class Rest : IGameState
{
    private GameStateMachine gsm;
    private Player player;
    

    public Rest(GameStateMachine gsm, Player player)
    {
        this.gsm = gsm;
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log("휴식 상태 진입");

        // 씬 전환
        GameSceneManager.Instance.ChangeScene(GameScenes.RestScene);

        // 씬 종료 이벤트 구독
        RestSceneController.OnFinished += FinishRest;
    }

    public void Update()
    {
        //Debug.Log("휴식상태 로직 처리중 ");
    }

    public void Exit()
    {
        // 이벤트 정리
        RestSceneController.OnFinished -= FinishRest;
    }

    // =========================
    // 씬에서 호출되는 종료 지점
    // =========================
    private void FinishRest()
    {
        Debug.Log("휴식 상태 종료");

        player.MentalHealth += 10;
       // time.AdvanceMonth();

        //엔딩 검사 +메인으로 돌아오기
        GameManager.Instance.OnActionStateFinished();
        GameSceneManager.Instance.ChangeScene(GameScenes.HomeScene);
    }
}
