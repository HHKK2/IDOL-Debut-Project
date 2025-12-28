using UnityEngine;

public class TimeCycleManager : AdolpSingleton<TimeCycleManager>
{
    public int currentSemester = 1; //분기(상반기/하반기)
    public int currentActionIndex = 1; // 학기 내 행동 인덱스 (1~4)

    //가능한 달
    private static readonly int[] FirstHalfMonths = { 1, 3, 4, 6 };
    private static readonly int[] SecondHalfMonths = { 7, 9, 10, 12 };

    public bool didComeBack;

    public GameStateMachine gameStateMachine;
    public Player player;

    public void AdvanceMonth()
    {
        if (currentActionIndex < 4)
        {
            currentActionIndex++; // 다음 달
        }
        else
        {
            EndSemester(); // 4달 끝 다음 학기
        }
    }

    public void EndSemester()
    {
        currentActionIndex = 1;
        currentSemester++;
        didComeBack = false;
    }


    //이번달은?
    public int GetCurrentMonth()
    {
        int index = currentActionIndex - 1; // 0~3

        if (IsFirstHalf())
            return FirstHalfMonths[index];
        else
            return SecondHalfMonths[index];
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

    //메인매뉴 UI에 표시할 정보들 TODO 수정 필요.

    /// <summary>
    /// 상반기 / 하반기 반환
    /// </summary>
    public bool IsFirstHalf()
    {
        return currentSemester % 2 == 1;
    }
    public string GetHalfYearString()
    {
        return IsFirstHalf() ? "상반기" : "하반기";
    }

    /// <summary>
    /// 메인메뉴 HUD에 표시할 날짜 문자열
    /// 예: "상반기 3월 2주차"
    /// </summary>
    public string GetCurrentDateString()
    {
        return $"{GetHalfYearString()} {GetCurrentMonth()}월";
    }

    /// 초기화 함수
    public void Reset()
    {
        currentSemester = 1;
        currentActionIndex = 1;
        didComeBack = false;
        repeatedNegative = 0;

        Debug.Log("[TIME RESET] Semester=1, MonthIndex=1");
    }
}