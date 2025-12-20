using UnityEngine;

public class Player
{
    private string name;
    private Gender gender;
    private int reputation; //대중 평판
    //private int stamina; // 체력
    private int money; // 돈
    private int fanNumber; // 팬 수
    //private int fanOpinion; //팬 민심
    private int mentalHealth; // 정신력
    private int singingAbility; // 가창력

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

    // public int Stamina
    // {
    //     get { return stamina; }
    //     set { stamina = value; }
    // }

    public int Reputation
    {
        get { return reputation; }
        set { reputation = value; }
    }

    public int Money
    {
        get { return money; }
        set { money = value; }
    }

    public int FanNumber
    {
        get { return fanNumber; }
        set { fanNumber = value; }
    }

    // public int FanOpinion
    // {
    //     get { return fanOpinion; }
    //     set { fanOpinion = value; }
    // }

    public int MentalHealth
    {
        get { return mentalHealth; }
        set { mentalHealth = value; }
    }

    public int SingingAbility
    {
        get { return singingAbility; }
        set { singingAbility = value; }
    }

}
