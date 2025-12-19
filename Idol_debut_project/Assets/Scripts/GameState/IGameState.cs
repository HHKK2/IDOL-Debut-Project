using UnityEngine;

public interface IGameState
{
    //각 게임 상태를 위한 인터페이스 GameStateMachine은 오직 IGameState에서만 접근함
    //GameManager은 현재 State를 통해 게임 흐름을 제어
    // 각 상태 전환 조건 정리
    // 각 상태의 로직 작성
    void Enter();
    void Update();
    void Exit();
}
