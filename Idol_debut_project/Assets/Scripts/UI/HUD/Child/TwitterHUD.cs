using System;
using Data;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class TwitterHUD : UIHUD
{
    enum GameObjects{
        TwitterContents,
    }
    
    [SerializeField] private TwitterDatabaseSO twitterDatabase;
    [Tooltip("트위터 랜덤으로 몇 개 띄울지")]
    [SerializeField] private int randomTwitCount = 3;

    [SerializeField] private GameObject twitterSlotPrefab;


    private bool initialized = false;
    private GameObject TwitterContents;
    
    private void Start()
    {
        if (initialized)
        {
            return;
        }
        
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        if (initialized)
        {
            return;
        }

        base.Init();
        
        Bind<GameObject>(typeof(GameObjects));
        TwitterContents = Get<GameObject>((int)GameObjects.TwitterContents);

        initialized = true;
    }
    
    
    public void Init(AudianceData.EAudianceFeeling audianceFeeling,  bool isReputationPositiveNumber)
    {
        if (!initialized)
        {
            EnsureInitialized();
        }

        int reputationStatus;
        if (isReputationPositiveNumber)
        {
            reputationStatus = 1;
        }
        else
        {
            reputationStatus = 0;
        }
        TwitterData[] twits = twitterDatabase.GetRandomTweets(audianceFeeling, reputationStatus, randomTwitCount);
        
        for (int i = 0; i < twits.Length; i++)
        {
            var twitterSlotGameObject = Instantiate(twitterSlotPrefab, TwitterContents.transform);
            twitterSlotGameObject.GetComponent<TwitterSlot>().Init(twits[i].name, twits[i].userID, twits[i].profilePic, twits[i].text);
        }
    }
    
    
}
