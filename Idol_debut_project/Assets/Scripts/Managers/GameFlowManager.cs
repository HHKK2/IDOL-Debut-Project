/// <summary>
/// 게임 전체 흐름과 엔딩 조건을 관리하는 매니저.
/// - 게임이 계속 가능한지 감시
/// - 엔딩 조건 충족 시 게임 종료 처리
/// </summary>

using UnityEngine;

public enum EndingType
{
    None, // 아직 엔딩이 아님 (진행 중)
    Normal, //12월이 됨 + 팬 5만명 안 됨
    Happy, //12월이 됨 + 팬 5만명 달성
    Bad, //멘탈이 터짐. 
    Wedding, //디스패치에 3번 걸림
    Superstar //팬 10만 명을 달성함. 
}
public class GameFlowManager
{

    public Player player;
    public TimeCycleManager time;

    public int dispatchCount = 0;

    public bool isGameEnded { get; private set; }
    public EndingType End { get; private set; } = EndingType.None;

    public void CheckEnding()
    {
        if (isGameEnded) return;

        //1. 배드 엔딩 : 멘탈이 0
        if (player.MentalHealth <= 0)
        {
            EndGame(EndingType.Bad);
            return;
        }
        //2. 웨딩 엔딩 : 디스패치에 3번
        if (dispatchCount >= 3)
        {
            EndGame(EndingType.Wedding);
            return;
        }
        //3. 슈퍼스타엔딩 : 팬을 10만 명 달성
        if (player.FanNumber >= 100_000 && time.currentMonth < 12)
        {
            EndGame(EndingType.Superstar);
            return;
        }
        //4. 노멀 엔딩 : 12월이 됨 + 5만이 안 됨
        //5. 해피 엔딩 : 12월이 됨 + 5만이 됨
        if (time.currentMonth >= 12)
        {
            if (player.FanNumber >= 50_000)
                EndGame(EndingType.Happy);
            else
                EndGame(EndingType.Normal);
        }
    }

    private void EndGame(EndingType ending)
    {
        isGameEnded = true;
        End = ending;

        Debug.Log($"게임 종료 - 엔딩 : {ending}");

        //TODO : 엔딩 UI /씬 전환
        //TODO : gamestatemachine 정지
    }

}