using UnityEngine;

public class Player
{
    private string name;
    private Gender gender;
    private int reputation; //대중 평판
    private int fanNumber; // 팬 수
    private int mentalHealth; // 정신력

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
        set
        {
            reputation = Mathf.Clamp(value, -100, 100);
        }
    }

    public int FanNumber
    {
        get { return fanNumber; }
        set
        {
            fanNumber = Mathf.Max(0, value);
        }
    }

    public int MentalHealth
    {
        get { return mentalHealth; }
        set
        {
            mentalHealth = Mathf.Clamp(value, 0, 100);
        }
    }
}
