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

    private void LoadSceneOnButtonPress(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber, LoadSceneMode.Single);
    }
}