using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// PersistentScene 전용 부트스트랩
/// - 싱글톤 매니저들이 있는 PersistentScene에서
/// - StartScene을 로드해 게임을 시작한다.
/// </summary>
public class PersistentSceneController : MonoBehaviour
{
    [SerializeField] private string startSceneName = "StartScene";

    private static bool isInitialized = false;

    private void Awake()
    {
        // PersistentScene이 중복 로드되는 것 방지
        if (isInitialized)
        {
            Destroy(gameObject);
            return;
        }

        isInitialized = true;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // StartScene이 아직 로드되지 않았다면 로드
        if (!SceneManager.GetSceneByName(startSceneName).isLoaded)
        {
            SceneManager.LoadScene(startSceneName);
        }
    }
}
