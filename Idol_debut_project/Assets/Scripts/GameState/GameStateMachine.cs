/// <summary>
/// GameStateMachine은 게임의 상태(State)를 바꿔줍니다.
/// 바꿔달라면 바꿔줌!
///
/// 역할:
/// - 현재 State를 유지한다.
/// - State 전환 시 Exit → Enter 호출 순서를 보장한다.
/// - 매 프레임 현재 State의 Update를 호출한다.
/// </summary>

using UnityEngine;

public class GameStateMachine
{
    private IGameState currentState;

    public void ChangeState(IGameState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;

        if (currentState != null)
        {
            currentState.Enter();
        }
    }

    public void Tick()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    public IGameState GetCurrentState()
    {
        return currentState;
    }
}
