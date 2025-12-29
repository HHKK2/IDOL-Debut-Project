using UnityEngine;

public static class DialogueTextFormatter 
{
    public static string ResolvePlayerTokens(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return raw;

        var gm = GameManager.Instance;
        if (gm == null || gm.player == null) return raw;

        var p = gm.player;

        string genderKor = p.Gender == Gender.FEMALE ? "걸" : "보이";

        string result = raw;
        result = result.Replace("{플레이어 이름}", p.Name);
        result = result.Replace("{플레이어 그룹이름}", p.GroupName);
        result = result.Replace("{플레이어 성별}", genderKor);

        return result;
    }
}
