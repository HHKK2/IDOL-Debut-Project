using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class SaveManager : AdolpSingleton<SaveManager>
{
    //Save Data 파일 JSON 으로 저장 및 불러오기
    // PlayerStatController 와 GameManager에서 필요한 정보만 가져와 기록
    //
    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public void Save()
    {
        Debug.Log($"Save Path: {SavePath}");
        
        SaveData data = new SaveData();

        var player = GameManager.Instance.player;
        data.playerName = player.Name;
        data.groupName = player.GroupName;
        data.reputation = player.Reputation;
        data.fanNumber = player.FanNumber;
        data.mentalHealth = player.MentalHealth;
        data.canDating = player.CanDating;

        var time = TimeCycleManager.Instance;
        data.currentSemester = time.currentSemester;
        data.currentActionIndex = time.currentActionIndex;
        data.didComeBack = time.didComeBack;
        data.repeatedNegative = time.repeatedNegative;

        var gm = GameManager.Instance;
        data.dispatchCount = gm.dispatchCount;
        data.isGameEnded = gm.isGameEnded;
        data.endingType = gm.End;

        data.tutorialCompleted = true;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        
        Debug.Log("[SAVE] successful");

    }

    public void Load()
    {
        if(!HasSave()) return;

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        var player = GameManager.Instance.player;
        player.Name = data.playerName;
        player.GroupName = data.groupName;
        player.Reputation = data.reputation;
        player.FanNumber = data.fanNumber;
        player.MentalHealth = data.mentalHealth;
        if(!data.canDating) player.DisableDating();

        var time = TimeCycleManager.Instance;
        time.currentSemester = data.currentSemester;
        time.currentActionIndex = data.currentActionIndex;
        time.didComeBack = data.didComeBack;
        time.repeatedNegative = data.repeatedNegative;

        var gm = GameManager.Instance;
        gm.dispatchCount = data.dispatchCount;
        
        Debug.Log("[LOAD] finished");
    }

    public bool LoadGame()
    {
        if (!HasSave()) return false;
        Load();
        return true;
    }

    public void SaveGame() => Save();
}
