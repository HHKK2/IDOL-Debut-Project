using UnityEngine;

public class Dating : IGameState
{
    private Player player;
    private GameStateMachine gsm;
    private DatingResult result;

    #region enum
    public enum DatingResult //데이트 결과
    {
        MentalUp, // 멘탈 + 30
        Dispatch, // 팬수 5분의 1 소멸, 멘탈 -20 (10프로)
        Breakup // 멘탈 -50, 앞으로 연애 비활성화 (5프로)
    }
    #endregion

    public Dating(GameStateMachine gsm, Player player)
    {
        this.gsm = gsm;
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log("연애 상태 진입");

        float roll = Random.value; // 랜덤 변수

        if (roll < 0.10f) //10% 디스패치
        {
            result = DatingResult.Dispatch;
        }
        else if (roll < 0.15f) //5% 결별
        {
            result = DatingResult.Breakup;
        }
        else
        {
            result = DatingResult.MentalUp;
        }

        // 결과 로그 (중요)
        Debug.Log($"[Dating] Result = {result}");

        // Scene에 데이터 전달 (성별 + 결과)
        DatingSceneController.SetContext(player.Gender, result);

        // Scene 종료 이벤트 구독
        DatingSceneController.OnFinished += FinishDating;

        // 씬 전환
        GameSceneManager.Instance.ChangeScene(GameScenes.DatingScene);
    }

    public void Update()
    {
        //Debug.Log("연애 상태 로직 처리중");
        //끝나면 gamestatemachine 쪽에서 exit 호출할 거임.
    }

    public void Exit()
    {
        Debug.Log("연애 상태 종료");

        DatingSceneController.OnFinished -= FinishDating;
    }

    private void FinishDating()
    {
        var stat = PlayerStatController.Instance;

        // === 결과 적용 ===
        switch (result)
        {
            case DatingResult.Dispatch:
                GameManager.Instance.dispatchCount++;

                int fanLoss = stat.FanNumber / 5;
                stat.ModifyFanNumber(-fanLoss);
                stat.ModifyMentalHealth(-20);
                break;
            case DatingResult.Breakup:
                stat.ModifyMentalHealth(-50);

                player.DisableDating();
                break;
            case DatingResult.MentalUp:
                stat.ModifyMentalHealth(+30);
                break;
        }

        //time.AdvanceMonth();

        // 흐름 복귀, 엔딩인지 체크 + 메인으로 돌아오기

        GameManager.Instance.CheckImmediateEnding();

        if (GameManager.Instance.isGameEnded)
            return;

        GameManager.Instance.OnActionStateFinished();
        // GameSceneManager.Instance.ChangeScene(GameScenes.HomeScene);
    }
}
