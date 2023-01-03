using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    // Scene order is as follows:
    // 0: Main Menu
    // 1: Online Menu
    // 2: Game Scene (Draft/Placement/Fight/GameOver)
    // 3: Tutorial Scene
    // 4: Lore Scene
    // 5: Credits Scene

    private void Awake()
    {
        SubscribeEvents();
    }

    public void LoadMainMenuScene()
    {
        LoadSceneOnButtonPress(0);
    }

    public void LoadOnlineMenuScene()
    {
        LoadSceneOnButtonPress(1);
    }

    public void LoadGameScene()
    {
        LoadSceneOnButtonPress(2);
    }

    public void LoadTutorialScene()
    {
        LoadSceneOnButtonPress(3);
    }

    public void LoadLoreScene()
    {
        LoadSceneOnButtonPress(4);
    }

    public void LoadCreditsScene()
    {
        LoadSceneOnButtonPress(5);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LoadSceneOnButtonPress(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber, LoadSceneMode.Single);

        // Shut down Server and Client if it is not the online menu or game scene
        if(sceneNumber != 1 && sceneNumber != 2)
        {
            GameObject onlineGameManager = GameObject.Find("OnlineGameManager");
            if (onlineGameManager)
            {
                Destroy(onlineGameManager);
            }
        }
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameEvents.OnGameStart += LoadGameScene;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGameStart -= LoadGameScene;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion
}