using UnityEngine;
using System;
using System.Threading.Tasks;
using Data;
using UnityEngine.SceneManagement;

public class JsonReader : AdolpSingleton<JsonReader>
{
    enum EReadMode{
        Twitter,
        None
    }
    
    [SerializeField] private TextAsset textAssetData;
    [SerializeField] private EReadMode eReadMode;
    
    
    private async void Start()
    {
        await ReadJsonAsync();
        
        Debug.Log("트위터 데이터 로드를 완료하여 데이터로더 객체를 파괴합니다.");
        
        Destroy(gameObject);
    }

    private async Task ReadJsonAsync()
    {
        switch (eReadMode)
        {
            case EReadMode.Twitter:
                //1. 구조체에 저장
                TwitterDataList loadedData = JsonUtility.FromJson<TwitterDataList>(textAssetData.text);
        
                //2. 데이터에 저장
                TwitterDatabase.Instance.SetData(loadedData.twitter); 
                break;
            default:
                Debug.Log("[TSVReader] 읽기모드를 선택해주세요");
                break;
        }
        
    }
}
