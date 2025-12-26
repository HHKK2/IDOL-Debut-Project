using UnityEngine;
using Data;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TwitterDatabase", menuName = "ScriptableObjects/TwitterDatabase")]
public class TwitterDatabaseSO : ScriptableObject
{
    private TwitterData[] twitterDataArray;

    public void SetData(TwitterData[] data)
    {
        twitterDataArray = data;
        
        //TODO: {플레이어이름} {그룹이룸}필드에 실제 플레이어 / 그룹 이름 넣기
    }
    
    /// <param name="reputationStatus">무대가 끝난 시점의 플레이어 평판. 양수인지, 음수인지 구별 (int).1: 양수, 0: 음수</param>
    /// <param name="count">랜덤으로 가져올 트위터 개수</param>
    public TwitterData[] GetRandomTweets(AudianceData.EAudianceFeeling feeling, int reputationStatus, int count)
    {
        // 해당 감정 && 평판의 데이터만 필터링
        List<TwitterData> filteredList = twitterDataArray.Where(i=>i.feeling==feeling && i.reputationStatus==reputationStatus)
            .ToList();

        if (filteredList.Count == 0)
        {
            Debug.LogError("조건에 맞는 트위터 데이터 없음!");
            return null;
        }

        if (filteredList.Count < count)
        {
            Debug.LogError($"조건에 맞는 트위터 데이터가 {count}개 미만임!");
            return null;
        }

        TwitterData[] result = filteredList
            .OrderBy(i=>Random.value)
            .Take(count)
            .ToArray();
        
        return result;
    }
}
