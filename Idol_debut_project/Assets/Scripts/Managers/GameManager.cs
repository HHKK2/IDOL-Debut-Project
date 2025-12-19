using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 게임의 전체 흐름을 관리하는 최상위 매니저
    // 현재 게임 상태 (IGameState)를 관리
    // GameStateMachie을 통해 상태 전환 제어
    // 각 State의 Enter, Update, Exit 호출 보장
    // 다른 시스템들이 현재 게임 상태를 조회할 수 있는 전역 진입점 제공
    
    // 책임 범위
    // 게임 시작 시 초기 State 설정
    // 상태 전환 요청을 받아 StateMachine에 전달
    // 전역 싱글톤 인스턴스 제공
    
    // 하지 말것
    // Player 스탯 직접 수정
    // UI 로직 직접 처리
    // Sound 판정 로직 처리
    // 개별 GameState 내부 로직 관여
    public static GameManager Instance;
    private GameStateMachine _stateMachine;

    void Awake()
    {
        Instance = this;
        _stateMachine = new GameStateMachine();
    }

    void Start()
    {
        _stateMachine.ChangeState(new Rest());
    }

    void Update()
    {
        _stateMachine.Tick();
    }

    public void ChangeState(IGameState newState)
    {
        _stateMachine.ChangeState(newState);
    }
}
