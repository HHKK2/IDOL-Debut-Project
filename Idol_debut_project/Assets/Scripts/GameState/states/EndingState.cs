using UnityEngine;

public class EndingState : IGameState
{
    private EndingType endingType;

    public EndingState(EndingType endingType)
    {
        this.endingType = endingType;
    }

    public void Enter()
    {
        Debug.Log($"엔딩 진입 : {endingType}");
        // TODO: 엔딩 UI 표시
        // TODO: 입력 막기
    }

    public void Update()
    {
        // 보통 아무것도 안 함
        // 또는 엔딩 화면에서 입력 대기
    }

    public void Exit()
    {
        // 엔딩에서는 보통 Exit 없음
    }
}
