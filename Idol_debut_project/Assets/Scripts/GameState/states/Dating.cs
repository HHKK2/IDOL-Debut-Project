using UnityEngine;

public class Dating : IGameState
{
    private Player player;
    private TimeCycleManager time;
    private GameFlowManager gameFlow;
    private DatingResult result;

    #region enum
    private enum DatingResult //데이트 결과
    {
        MentalUp, // 멘탈 + 30
        Dispatch, // 팬수 5분의 1 소멸, 멘탈 -20 (10프로)
        Breakup // 멘탈 -50, 앞으로 연애 비활성화 (5프로)
    }
    #endregion

    public Dating(Player player, TimeCycleManager time, GameFlowManager gameFlow)
    {
        this.player = player;
        this.time = time;
        this.gameFlow = gameFlow;
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
    }

    public void Update()
    {
        Debug.Log("연애 상태 로직 처리중");
        //끝나면 gamestatemachine 쪽에서 exit 호출할 거임.
    }

    public void Exit()
    {
        Debug.Log("연애 상태 종료");

        switch (result)
        {
            case DatingResult.Dispatch:
                player.FanNumber -= player.FanNumber / 5; //5분의 1이 날아갑니다. shit!
                player.MentalHealth -= 20;
                break;

            case DatingResult.Breakup:
                player.MentalHealth -= 50;
                player.DisableDating(); //이별하셨나요? 더 이상 연애를 못 합니다.
                break;

            case DatingResult.MentalUp:
                player.MentalHealth += 30;
                break;
        }
        time.AdvanceWeek();

        //엔딩 체크
        gameFlow.OnActionFinished();
    }
}
