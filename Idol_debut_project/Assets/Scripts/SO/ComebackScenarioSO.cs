using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Comeback/Scenario")]
public class ComebackScenarioSO : ScriptableObject
{
    public List<ComebackEntry> entries;

    public ComebackSongData GetSong(int comebackIndex, Gender gender)
    {
        if (comebackIndex < 0 || comebackIndex >= entries.Count)
        {
            Debug.LogError("컴백 인덱스 초과");
            return null;
        }

        return entries[comebackIndex].GetSongByGender(gender);
    }
}
