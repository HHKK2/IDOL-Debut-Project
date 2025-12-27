using UnityEngine;

/// <summary>
/// 게임 전체 흐름과 엔딩 조건을 관리하는 최상위 매니저
///
/// - 게임 시작 제어
/// - 행동 시작/종료 관리
/// - 엔딩 조건 판정
/// - GameStateMachine 제어
/// </summary>

public enum EndingType
{
    None, // 아직 엔딩이 아님 (진행 중)
    Normal, //12월이 됨 + 팬 5만명 안 됨
    Happy, //12월이 됨 + 팬 5만명 달성
    Bad, //멘탈이 터짐. 
    Wedding, //디스패치에 3번 걸림
    Superstar //팬 10만 명을 달성함. 
}

public class GameManager : AdolpSingleton<GameManager>
{
    private GameStateMachine gsm;

    [SerializeField] private Player player;
    [SerializeField] private TimeCycleManager time;

    // =========================
    // 엔딩 관련
    // =========================

    public int dispatchCount = 0;
    private const int FINAL_SEMESTER = 6;

    public bool isGameEnded { get; private set; }
    public EndingType End { get; private set; } = EndingType.None;

    protected override void Awake()
    {
        base.Awake();
        gsm = new GameStateMachine();
    }


    private void Update()
    {
        gsm.Tick();
    }

    // =========================
    // 게임 시작
    // =========================

    /// <summary>
    /// 튜토리얼 종료 후 호출됨
    /// </summary>
    public void StartGame()
    {
        isGameEnded = false;
        End = EndingType.None;

        // 첫 상태: 행동 선택
        gsm.ChangeState(
            new ChooseActionState(gsm, player, time)
        );
    }

    // =========================
    // 행동 시작
    // =========================

    public void StartAction(ActivityType activity)
    {
        switch (activity)
        {
            case ActivityType.Practice:
                gsm.ChangeState(new Training(gsm, player, time));
                break;
            case ActivityType.Comeback:
                gsm.ChangeState(new ComeBack(gsm, player, time));
                break;
            case ActivityType.FanService:
                gsm.ChangeState(new FanService(gsm, player, time));
                break;
            case ActivityType.Rest:
                gsm.ChangeState(new Rest(gsm, player, time));
                break;
            case ActivityType.Dating:
                gsm.ChangeState(new Dating(gsm, player, time));
                break;
        }
    }

    // =========================
    // 행동 종료
    // =========================

    /// <summary>
    /// 하나의 행동(GameState)이 끝났을 때 호출됨
    /// </summary>
    public void OnActionStateFinished()
    {
        CheckEnding();

        if (isGameEnded)
            return;

        // 엔딩이 아니면 다시 행동 선택
        gsm.ChangeState(
            new ChooseActionState(gsm, player, time)
        );
    }

    // =========================
    // 엔딩 판정
    // =========================

    public void CheckEnding()
    {
        if (isGameEnded) return;

        //1. 배드 엔딩 : 멘탈이 0
        if (player.MentalHealth <= 0)
        {
            EndGame(EndingType.Bad);
            return;
        }
        //2. 웨딩 엔딩 : 디스패치에 3번
        if (dispatchCount >= 3)
        {
            EndGame(EndingType.Wedding);
            return;
        }
        //3. 슈퍼스타엔딩 : 팬을 10만 명 달성
        if (player.FanNumber >= 100_000 && time.currentSemester < FINAL_SEMESTER)
        {
            EndGame(EndingType.Superstar);
            return;
        }
        //4. 노멀 엔딩 : 12월이 됨 + 5만이 안 됨
        //5. 해피 엔딩 : 12월이 됨 + 5만이 됨
        if (time.currentSemester >= FINAL_SEMESTER)
        {
            if (player.FanNumber >= 50_000)
                EndGame(EndingType.Happy);
            else
                EndGame(EndingType.Normal);
        }
    }

    private void EndGame(EndingType ending)
    {
        isGameEnded = true;
        End = ending;

        Debug.Log($"게임 종료 - 엔딩 : {ending}");

        // 엔딩 상태로 전환
        gsm.ChangeState(new EndingState(ending));
    }
}
