using UnityEngine;

public class HomeSceneController : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("HomeScene START → StartGame");
        GameManager.Instance.StartGame();
    }
}
