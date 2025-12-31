using UnityEngine;
using Data;
using System.Linq;
using System.Collections.Generic;

public class TwitterDatabase : AdolpSingleton<TwitterDatabase>
{    
    [SerializeField] private TextAsset twitterJson;

    public TwitterData[] twitterDataArray;

    public TwitterData[] TwitterDataArray
    {
        get { return twitterDataArray; }
    }
    
    private bool isLoaded = false;


    public void SetData(TwitterData[] data)
    {
        twitterDataArray = data;

        if (GameManager.Instance.player.Name == null || GameManager.Instance.player.GroupName == null)
        {
            return;
        }
        
        for (int i = 0; i < twitterDataArray.Length; i++)
        {
            twitterDataArray[i].text = twitterDataArray[i].text.Replace("{플레이어이름}", GameManager.Instance.player.Name);
            twitterDataArray[i].text = twitterDataArray[i].text.Replace("{그룹이름}", GameManager.Instance.player.GroupName);
        }
    }

    public void SetPlayerName()
    {
        for (int i = 0; i < twitterDataArray.Length; i++)
        {
            twitterDataArray[i].name = twitterDataArray[i].name.Replace("{플레이어이름}", GameManager.Instance.player.Name);
            twitterDataArray[i].text = twitterDataArray[i].text.Replace("{플레이어이름}", GameManager.Instance.player.Name);
        }
    }
    
    public void SetGroupName()
    {
        for (int i = 0; i < twitterDataArray.Length; i++)
        {
            twitterDataArray[i].name = twitterDataArray[i].name.Replace("{그룹이름}", GameManager.Instance.player.GroupName);
            twitterDataArray[i].text = twitterDataArray[i].text.Replace("{그룹이름}", GameManager.Instance.player.GroupName);
        }
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
