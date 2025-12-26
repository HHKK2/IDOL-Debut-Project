using UnityEngine;
using Data;

public class Player
{
    private string name;
    private Gender gender;
    private int reputation; //대중 평판
    private int fanNumber; // 팬 수
    private int mentalHealth; // 정신력
    private string groupName; //그룹명
    private bool canDating = true; //연애 가능 여부 플래그 변수

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public Gender Gender
    {
        get { return gender; }
        set { gender = value; }
    }
    public int Reputation
    {
        get { return reputation; }
        set { reputation = value; }
    }

    public int FanNumber
    {
        get { return fanNumber; }
        set { fanNumber = value; }
    }

    public int MentalHealth
    {
        get { return mentalHealth; }
        set { mentalHealth = value; }
    }

    public string GroupName
    {
        get { return groupName; }
        set { groupName = value; }
    }

    public bool CanDating
    {
        get { return canDating; }
        set { canDating = value; }
    }

    //결별 시 영구적으로 연애 비활성화 
    public void DisableDating()
    {
        CanDating = false;
    }

    /// <summary>
    /// InputHUD에서 받은 PlayerInfoData를
    /// 실제 Player 데이터로 반영하는 메서드
    /// (게임 시작 시 단 1회 호출)
    /// </summary>
    public void ApplyPlayerInfo(PlayerInfoData data)
    {
        Name = data.name;
        groupName = data.groupName;
        //Gender = data.gender; gender를 enum 대로 통일해야 할듯... TODO

        // 초기값 세팅 (기획에 맞게 조정)
        FanNumber = 0;
        MentalHealth = 100;
    }

    /// <summary>
    /// 멘탈을 0~1 값으로 변환 (UI용)
    /// </summary>
    public float GetMentalRatio()
    {
        return Mathf.Clamp01(MentalHealth / 100f);
    }

}
