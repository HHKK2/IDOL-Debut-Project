using UnityEngine;

public class ComeBack : IGameState
{
    Player player;
    private GameStateMachine gsm;

    private int stageScore;
    private int bonus;

    public ComeBack(GameStateMachine gsm, Player player)
    {
        this.gsm = gsm;
        this.player = player;
    }

    //10분 동안만 연습이 가능합니다. 
    private float practiceTimer = 0f;
    private const float PRACTICE_DURATION = 600f; // 10분

    private enum ComeBackPhase
    {
        Practice,
        Stage,
        Done
    }

    private ComeBackPhase phase;


    public void Enter()
    {
        Debug.Log("컴백 상태 진입");

        int stageScore = CalculateStageScore(); //TODO : 나중에 함수를 받아와야겠지..
        bonus = GetStageBonus(stageScore);

        phase = ComeBackPhase.Practice;
        practiceTimer = 0f;

        // 씬 전환
        GameSceneManager.Instance.ChangeScene(GameScenes.ComebackScene);

        // 씬 종료 이벤트 구독
        ComebackSceneController.OnFinished += FinishComeBack;
    }

    public void Update()
    {
        //Debug.Log("컴백 상태 로직 처리");

        if (phase == ComeBackPhase.Practice)
        {
            practiceTimer += Time.deltaTime;

            if (practiceTimer >= PRACTICE_DURATION)
            {
                phase = ComeBackPhase.Stage;
                StartStage();
            }
        }
    }

    public void Exit()
    {
        Debug.Log("컴백 상태 종료");

        //이벤트 해제
        ComebackSceneController.OnFinished -= FinishComeBack;

    }

    private void FinishComeBack()
    {
        var time = TimeCycleManager.Instance;
        
        Debug.Log("[COMEBACK FINISH] 결과 적용");

        // 1. 평판 변화
        int beforeReputation = player.Reputation;
        player.Reputation += bonus;
        int reputationDelta = player.Reputation - beforeReputation;

        // 2. 팬 수 변화 : 평판 증감량 * 100
        player.FanNumber += reputationDelta * 100;

        // 3. 멘탈 변화
        if (reputationDelta > 0)
        {
            // 평판이 늘어났다면
            player.MentalHealth += bonus * 3;
            time.ResetNegativeReputation();
        }
        else if (reputationDelta < 0)
        {
            int absBonus = Mathf.Abs(bonus);
            time.IncreaseNegativeReputation();

            if (time.repeatedNegative >= 3)
            {
                // 연속 3회 이상 평판 -
                player.MentalHealth -= absBonus * 5;
            }
            else
            {
                // 일반 평판 -
                player.MentalHealth -= absBonus * 3;
            }
        }

        // 4. 주 종료 + 컴백 완료
        time.didComeBack = true;

        // 5. 엔딩 체크 + 메인메뉴로 돌아오기
        GameManager.Instance.OnActionStateFinished();
        GameSceneManager.Instance.ChangeScene(GameScenes.HomeScene);
    }

    private int GetStageBonus(int score) //TODO : 무대평가 후 score 받아와야 함. 
    {
        if (score >= 100) return 5;
        else if (score >= 90) return 4;
        else if (score >= 80) return 3;
        else if (score >= 70) return 2;
        else if (score >= 60) return 1;
        else if (score >= 50) return 0;

        // 0 ~ 40 전부 음수 처리
        else if (score >= 40) return -1;
        else if (score >= 30) return -2;
        else if (score >= 20) return -3;
        else if (score >= 10) return -4;
        else return -5;

    }

    private void StartStage()
    {
        Debug.Log("무대 시작");
        phase = ComeBackPhase.Stage;
    }

    private int CalculateStageScore()
    {
        //컴파일 제거 용 (나중에 삭제할 것.)
        return 1;
    }


}