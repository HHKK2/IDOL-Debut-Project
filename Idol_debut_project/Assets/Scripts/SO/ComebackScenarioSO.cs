using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Comeback/Scenario")]
public class ComebackScenarioSO : ScriptableObject
{
    public List<ComebackEntry> entries;

    /// Practice에서 사용
    public AudioClip GetPracticeClip(int comebackIndex, Gender gender)
    {
        return GetSong(comebackIndex, gender)?.GetPracticeClip();
    }

    /// Comeback에서 사용 + 사용 처리
    public AudioClip GetComebackClipAndMarkUsed(int comebackIndex, Gender gender)
    {
        var song = GetSong(comebackIndex, gender);
        if (song == null) return null;

        song.MarkUsedInComeback();
        return song.GetComebackClip();
    }

    /// 외부에서 사용 여부 확인
    public bool IsSongUsedInComeback(int comebackIndex, Gender gender)
    {
        var song = GetSong(comebackIndex, gender);
        return song != null && song.UsedInComeback;
    }

    public ComebackSongData GetSong(int comebackIndex, Gender gender)
    {
        if (comebackIndex < 0 || comebackIndex >= entries.Count)
        {
            Debug.LogError("컴백 인덱스 초과");
            return null;
        }

        return entries[comebackIndex].GetSongByGender(gender);
    }

    /// 최초 0번 곡은 이미 컴백 사용 처리
    public void InitDefaultUsedState()
    {
        if (entries == null || entries.Count == 0) return;

        entries[0].maleSong?.MarkUsedInComeback();
        entries[0].femaleSong?.MarkUsedInComeback();
    }
}
