using UnityEngine;

public class Training : IGameState
{
    private GameStateMachine gsm;
    private Player player;

    public Training(GameStateMachine gsm, Player player)
    {
        this.gsm = gsm;
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log("연습 상태 진입");

        // 씬 전환
        GameSceneManager.Instance.ChangeScene(GameScenes.PracticeScene);

        // 씬 종료 이벤트 구독
        PracticeSceneController.OnFinished += FinishTraining;
    }

    public void Update()
    {
        //Debug.Log("연습 상태 로직 처리중");
    }

    public void Exit()
    {
        Debug.Log("연습 상태 종료");
        // 이벤트 정리만
        PracticeSceneController.OnFinished -= FinishTraining;
    }

    private void FinishTraining()
    {
        Debug.Log("[TRAINING FINISH] 결과 적용");

        // 1. 시간 진행
        // time.AdvanceMonth();

        //즉각 엔딩 검사. 
        GameManager.Instance.CheckImmediateEnding();
        if (GameManager.Instance.isGameEnded)
            return;
        //다음 상태로 복귀
        GameManager.Instance.OnActionStateFinished();
        //메인으로 돌아오기
    }
}
