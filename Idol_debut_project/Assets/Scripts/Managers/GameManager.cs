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

    public Player player { get; private set; }
    public TimeCycleManager time { get; private set; }

    // =========================
    // 엔딩 관련
    // =========================

    public int dispatchCount = 0;
    private const int FINAL_SEMESTER = 6;

    public bool isGameEnded { get; private set; }
    public EndingType End { get; private set; } = EndingType.None;

    //컴백 곡 캐시 관련
    public ComebackScenarioSO comebackScenario;   // 인스펙터에서 연결
    public ComebackSongData CurrentComebackSong { get; private set; }  // 현재 컴백곡 캐시


    //isLoad 사용 여부
    public bool IsLoadedGame { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        // 게임 전역 데이터 생성
        player = new Player();
        time = TimeCycleManager.Instance;


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

        //time.Reset();   // timecycle 초기화


        // 로드한 게임이 아닐시에만 (aka 첫 시작 게임) 플레이어 스탯 셋팅
        if (!IsLoadedGame)
        {
            time.Reset();

            player.Reputation = 10;
            player.FanNumber = 4000;     // 예시 TODO : 감자씨! 초기 스탯을 바꾸세요.
            player.MentalHealth = 100;

            ResetComebackSongsExceptTutorial(); //컴백 여부 초기화
        }

        // 첫 상태: 행동 선택
        gsm.ChangeState(
            new ChooseActionState(gsm)
        );
        ClearLoadedGame();

    }

    public void EnterHome()
    {
        if (isGameEnded)
            return;

        gsm.ChangeState(new ChooseActionState(gsm));
    }


    // =========================
    // 행동 시작
    // =========================

    public void StartAction(ActivityType activity)
    {
        switch (activity)
        {
            case ActivityType.Practice:
                gsm.ChangeState(new Training(gsm, player));
                GameSceneManager.Instance.ChangeScene(GameScenes.PracticeScene);
                break;
            case ActivityType.Comeback:
                gsm.ChangeState(new ComeBack(gsm, player));
                GameSceneManager.Instance.ChangeScene(GameScenes.ComebackScene);
                break;
            case ActivityType.FanService:
                gsm.ChangeState(new FanService(gsm, player));
                GameSceneManager.Instance.ChangeScene(GameScenes.FanServiceScene);
                break;
            case ActivityType.Rest:
                gsm.ChangeState(new Rest(gsm, player));
                GameSceneManager.Instance.ChangeScene(GameScenes.RestScene);
                break;
            case ActivityType.Dating:
                gsm.ChangeState(new Dating(gsm, player));
                GameSceneManager.Instance.ChangeScene(GameScenes.DatingScene);
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
        Debug.Log($"[GM] playerRef={player.GetHashCode()} mental={player.MentalHealth}");

        CheckEnding();
        if (isGameEnded)
            return;

        time.AdvanceMonth();

        // 상태 종료 직전 Player 상태 로그
        Debug.Log(
            $"[STATE END]\n" +
            $"Reputation: {player.Reputation}\n" +
            $"FanNumber: {player.FanNumber}\n" +
            $"MentalHealth: {player.MentalHealth}\n" +
            $"Semester: {time.currentSemester}, MonthIndex: {time.currentActionIndex}"
        );

        SaveManager.Instance.SaveGame();

        // 엔딩이 아니면 다시 행동 선택
        gsm.ChangeState(
            new ChooseActionState(gsm)
        );
    }

    // =========================
    // 엔딩 판정 - 끝날 때
    // =========================

    public void CheckEnding()
    {
        if (isGameEnded) return;

        // //1. 배드 엔딩 : 멘탈이 0
        // if (player.MentalHealth <= 0)
        // {
        //     EndGame(EndingType.Bad);
        //     return;
        // }
        // //2. 웨딩 엔딩 : 디스패치에 3번
        // if (dispatchCount >= 3)
        // {
        //     EndGame(EndingType.Wedding);
        //     return;
        // }
        // //3. 슈퍼스타엔딩 : 팬을 10만 명 달성
        // if (player.FanNumber >= 100_000)
        // {
        //     EndGame(EndingType.Superstar);
        //     return;
        // }
        //4. 노멀 엔딩 : 12월이 됨 + 5만이 안 됨
        //5. 해피 엔딩 : 12월이 됨 + 5만이 됨
        bool isFinalSemester = time.currentSemester == FINAL_SEMESTER;
        bool isEndOfSemester = time.currentActionIndex == 4;

        if (isFinalSemester && isEndOfSemester)
        {
            if (player.FanNumber >= 50_000)
                EndGame(EndingType.Happy);
            else
                EndGame(EndingType.Normal);
        }
    }

    //엔딩 판정 - 바로.
    public void CheckImmediateEnding()
    {
        if (isGameEnded) return;

        if (player.MentalHealth <= 0)
        {
            EndGame(EndingType.Bad);
            return;
        }

        if (dispatchCount >= 3)
        {
            EndGame(EndingType.Wedding);
            return;
        }

        if (player.FanNumber >= 100_000)
        {
            EndGame(EndingType.Superstar);
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

    public void MarkLoadedGame()
    {
        IsLoadedGame = true;
    }

    public void ClearLoadedGame()
    {
        IsLoadedGame = false;

    }

    public void ResumeFromLoad()
    {
        isGameEnded = false;
        if (gsm == null)
        {
            gsm = new GameStateMachine();
        }
        gsm.ChangeState(new ChooseActionState(gsm));
    }

    //현재 컴백 곡 저장할 곳 만들기
    public void SetCurrentComebackSong(ComebackSongData song)
    {
        CurrentComebackSong = song;
    }

    //시작할 때, 노래들이 컴백했다는 것을 초기화시킵니다.
    private void ResetComebackSongsExceptTutorial()
    {
        if (comebackScenario == null)
        {
            Debug.LogWarning("[GameManager] comebackScenario is null");
            return;
        }

        foreach (var entry in comebackScenario.entries)
        {
            // order == 0 → 튜토리얼 곡이므로 스킵
            if (entry.order == 0)
                continue;

            if (entry.maleSong != null)
                entry.maleSong.ResetUsedInComeback();

            if (entry.femaleSong != null)
                entry.femaleSong.ResetUsedInComeback();
        }

        Debug.Log("[GameManager] Comeback songs reset (except tutorial)");
    }


}
