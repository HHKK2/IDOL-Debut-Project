using UnityEngine;

public class TimeCycleManager : MonoBehaviour
{
    public int currentMonth = 1;
    public int currentWeek = 1;

    public GameStateMachine gameStateMachine;
    public Player player;

    public void AdvanceWeek()
    {
        currentWeek++;
    }

    public void EndMonth()
    {
        currentWeek = 1;
        currentMonth++;
    }
}