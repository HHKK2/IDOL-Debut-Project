using UnityEngine;

public class TimeCycleManager : MonoBehaviour
{
    public int currentMonth = 1;
    public int currentWeek = 1;
    public bool didComeBack;

    public GameStateMachine gameStateMachine;
    public Player player;

    public void AdvanceWeek()
    {
        currentWeek++;

        if(currentWeek > 4)
        {
            EndMonth();
        }
    }

    public void EndMonth()
    {
        currentWeek = 1;
        currentMonth++;
        didComeBack = false;
    }

    //연속 평판
    public int repeatedNegative = 0;

    public void ResetNegativeReputation()
    {
        repeatedNegative = 0;
    }

    public void IncreaseNegativeReputation()
    {
        repeatedNegative++;
    }
}