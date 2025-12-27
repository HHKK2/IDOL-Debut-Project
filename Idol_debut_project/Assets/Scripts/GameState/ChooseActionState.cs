/// <summary>
/// 이번 주에 어떤 행동을 할지 선택하는 State.
/// 
/// 역할:
/// - 행동 선택 UI를 띄운다.
/// - 마지막 주(4주차)인데 컴백을 하지 않았다면 강제로 컴백 State로 전환한다.
///
/// 책임:
/// - 행동 선택 로직 및 강제 규칙 처리
///
/// 책임 아님:
/// - 실제 행동 결과 처리
/// - 시간 진행 (AdvanceWeek)
/// </summary

using UnityEngine;

public class ChooseActionState : IGameState
{
    GameStateMachine gsm;
    Player player;
    TimeCycleManager time;
    MainMenuHUD hud;

    // 할 수 있는 행동들
    private bool canComeBack;
    private bool canPractice;
    private bool canDating;
    private bool canRest;
    private bool canFanService;

    public ChooseActionState(GameStateMachine gsm, Player player, TimeCycleManager time)
    {
        this.gsm = gsm;
        this.player = player;
        this.time = time;
    }

    public void Enter()
    {
        Debug.Log("행동 선택 상태 진입");

        //UImanager 참조
        hud = UIManager.Instance.HUDList.Find(h => h is MainMenuHUD) as MainMenuHUD;

        if (hud == null)
            hud = UIManager.Instance.ShowHUDUI<MainMenuHUD>();

        //상단 정보 갱신(몇 분기 몇 월 / 팬수 / 멘탈 / 이름)
        hud.Init(
            date: time.GetCurrentDateString(),      // TimeCycleManager가 책임
            groupName: player.GroupName,             // InputHUD → Player에 이미 들어간 값
            fanNum: player.FanNumber.ToString(),     // 현재 스탯
            mental: player.GetMentalRatio(),          // 0~1로 변환된 값
            name: player.Name                        // InputHUD에서 받은 값
        );


        //상/하반기가 끝날텐데 컴백을 안 하셨다고요? 컴백을 하셔야겠네요.

        bool isLastMonth = (time.currentActionIndex == 4);
        bool Comeback = time.didComeBack;

        // 기본값 세팅
        canPractice = true;
        canDating = player.CanDating; // 연애 비활성화 반영
        canRest = true;
        canFanService = true;
        canComeBack = true;

        if (isLastMonth && !Comeback)
        {
            canComeBack = true; //컴백만 가능합니다.
            canPractice = false;
            canDating = false;
            canRest = false;
            canFanService = false;

            hud.MustComebackStarted(); //컴백 제외 UI 버튼 비활성화
        }
        else if (Comeback) //이미 컴백을 했으면, 이번 분기엔 컴백 불가.
        {
            canComeBack = false;
        }

        // ===== 이벤트 연결 =====
        hud.ClickedLiveButton += OnLive;
        hud.ClickedPracticeButton += OnPractice;
        hud.ClickedDatingButton += OnDating;
        hud.ClickedRestButton += OnRest;
        hud.ClickedComebackButton += OnComeback;
    }

    public void Update()
    {

    }

    public void Exit()
    {
        if (hud == null) return;

        hud.ClickedLiveButton -= OnLive;
        hud.ClickedPracticeButton -= OnPractice;
        hud.ClickedDatingButton -= OnDating;
        hud.ClickedRestButton -= OnRest;
        hud.ClickedComebackButton -= OnComeback;

        //UI 비활성화 버튼 초기화
        hud.ResetActionButtons();
    }

    //UI 담당 함수들
    private void OnLive()
    {
        if (!canFanService) return;
        GameManager.Instance.StartAction(ActivityType.FanService); //게임 매니저가 state 변경을 해줄 거임.
    }

    private void OnPractice()
    {
        if (!canPractice) return;
        GameManager.Instance.StartAction(ActivityType.Practice); //게임 매니저가 state 변경을 해줄 거임.
    }

    private void OnDating()
    {
        if (!canDating) return;
        GameManager.Instance.StartAction(ActivityType.Dating); //게임 매니저가 state 변경을 해줄 거임.
    }

    private void OnRest()
    {
        if (!canRest) return;
        GameManager.Instance.StartAction(ActivityType.Rest); //게임 매니저가 sate 변경을 해줄 거임.
    }

    private void OnComeback()
    {
        if (!canComeBack) return;
        GameManager.Instance.StartAction(ActivityType.Comeback); //게임 매니저가 state 변경을 해줄 거임. 
    }

}


