using UnityEngine;

public class HomeSceneController : MonoBehaviour
{
    private void Start()
    {
        var gm = GameManager.Instance;
        if (gm.IsLoadedGame)
        {
            Debug.Log("HomeScene Start -> load game continue");
            gm.ClearLoadedGame();
            gm.ResumeFromLoad();
        }
        else
        {
            Debug.Log("HomeScene START → StartGame");
            gm.EnterHome();
        }
        
    }
}
