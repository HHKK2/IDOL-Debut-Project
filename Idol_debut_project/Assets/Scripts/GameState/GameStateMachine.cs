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
        currentState.Enter();
    }

    public void Tick()
    {
        if (currentState == null)
        {
            currentState.Exit();
        }
    }

    public IGameState GetCurrentState()
    {
        return currentState;
    }
}
