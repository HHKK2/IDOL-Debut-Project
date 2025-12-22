using UnityEngine;

public class Player
{
    private string name;
    private Gender gender;
    private int reputation; //대중 평판
    private int fanNumber; // 팬 수
    private int mentalHealth; // 정신력
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
}
