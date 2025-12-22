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

        //상/하반기가 끝날텐데 컴백을 안 하셨다고요? 컴백을 하셔야겠네요.

        bool isLastWeek = (time.currentWeek == 4);
        bool Comeback = time.didComeBack;

        // 기본값 세팅
        canPractice = true;
        canDating = player.CanDating; // 연애 비활성화 반영
        canRest = true;
        canFanService = true;
        canComeBack = true;

        if (isLastWeek && !Comeback)
        {
            canComeBack = true; //컴백만 가능합니다.
            canPractice = false;
            canDating = false;
            canRest = false;
            canFanService = false;
        }
        else if (Comeback) //이미 컴백을 했으면, 이번 분기엔 컴백 불가.
        {
            canComeBack = false;
        }

        //TODO : UI에 값들을 넘겨야 함.
    }

    public void Update()
    {

    }

    public void Exit()
    {

    }
}


