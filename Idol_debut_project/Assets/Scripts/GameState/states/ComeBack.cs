using UnityEngine;

public class ComeBack : IGameState
{
    Player player;
    private GameStateMachine gsm;

    private int stageScore;
    private int bonus;
    private ComebackSongData currentSong; //이번 컴백 노래.

    public ComeBack(GameStateMachine gsm, Player player)
    {
        this.gsm = gsm;
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log("컴백 상태 진입");

        var time = TimeCycleManager.Instance;

        // 1) 이번 컴백 인덱스(0-base).
        int comebackIndex = time.comebackCount;

        // 2) 성별에 맞는 곡 선택
        var scenario = GameManager.Instance.comebackScenario;
        if (scenario == null)
        {
            Debug.LogError("[ComeBack] comebackScenario가 GameManager에 연결되지 않음");
            return;
        }

        currentSong = scenario.GetSong((comebackIndex + 1), player.Gender);
        if (currentSong == null)
        {
            Debug.LogError($"[ComeBack] 곡 선택 실패 index={comebackIndex} gender={player.Gender}");
            return;
        }

        // 3) GameManager에 캐시 (SceneController가 이걸 읽는다)
        GameManager.Instance.SetCurrentComebackSong(currentSong);

        // 4) 로그 
        Debug.Log($"[ComeBack] SelectedSong order={comebackIndex} id={currentSong.songId} title={currentSong.title} concept={currentSong.concept} gender={player.Gender}");



        stageScore = CalculateStageScore(); //TODO : 나중에 함수를 받아와야겠지..
        bonus = GetStageBonus(stageScore);

        // 씬 전환
        GameSceneManager.Instance.ChangeScene(GameScenes.ComebackScene);

        // 씬 종료 이벤트 구독
        ComebackSceneController.OnFinished += FinishComeBack;
    }

    public void Update()
    {
        //Debug.Log("컴백 상태 로직 처리");
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

        // =========================
        // 1. 평판 변화 (-100 ~ 100)
        // =========================
        int beforeReputation = player.Reputation;

        int newReputation = player.Reputation + bonus;
        player.Reputation = Mathf.Clamp(newReputation, -100, 100);

        int reputationDelta = player.Reputation - beforeReputation;

        // =========================
        // 2. 팬 수 변화 (0 ~ 무한)
        // =========================
        int fanDelta = reputationDelta * 1000 + 1000;
        player.FanNumber = Mathf.Max(0, player.FanNumber + fanDelta);

        // =========================
        // 3. 멘탈 변화 (0 ~ 100)
        // =========================
        int mentalDelta = 0;

        if (reputationDelta > 0)
        {
            mentalDelta = bonus * 3;
            time.ResetNegativeReputation();
        }
        else if (reputationDelta < 0)
        {
            int absBonus = Mathf.Abs(bonus);
            time.IncreaseNegativeReputation();

            mentalDelta = (time.repeatedNegative >= 3)
                ? -absBonus * 5
                : -absBonus * 3;
        }

        player.MentalHealth = Mathf.Clamp(
            player.MentalHealth + mentalDelta,
            0,
            100
        );

        // =========================
        // 4. 주 종료 + 컴백 완료
        // =========================
        time.didComeBack = true;
        time.CompleteComeback();

        // =========================
        // 5. 엔딩 체크
        // =========================
        GameManager.Instance.CheckImmediateEnding();
        if (GameManager.Instance.isGameEnded)
            return;

        GameManager.Instance.OnActionStateFinished();
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

    private int CalculateStageScore()
    {
        //컴파일 제거 용 (나중에 삭제할 것.) TODO stage 결과에서 받아오기
        return 1;
    }


}