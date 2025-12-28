[System.Serializable]
public class SaveData
{
    // 튜토리얼
    public bool tutorialCompleted;

    // Player
    public string playerName;
    public string groupName;
    public int reputation;
    public int fanNumber;
    public int mentalHealth;
    public bool canDating;

    // Time
    public int currentSemester;
    public int currentActionIndex;
    public bool didComeBack;
    public int repeatedNegative;

    // GameManager
    public int dispatchCount;
    public bool isGameEnded;
    public EndingType endingType;
}