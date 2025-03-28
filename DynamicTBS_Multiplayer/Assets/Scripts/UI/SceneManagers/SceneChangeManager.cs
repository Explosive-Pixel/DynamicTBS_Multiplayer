using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    MAIN_MENU = 0,
    ONLINE_MENU = 1,
    OFFLINE_MENU = 2,
    GAME = 3,
    TUTORIAL = 4,
    HALL_OF_FAME = 5,
    CREDITS = 6
}

public class SceneChangeManager : MonoBehaviour
{
    private Scene currentScene = Scene.MAIN_MENU;
    public Scene CurrentScene { get { return currentScene; } }

    #region SingletonImplementation
    public static SceneChangeManager Instance { set; get; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SubscribeEvents();
        }
    }
    #endregion

    public void LoadScene(Scene scene)
    {
        currentScene = scene;

        int sceneNumber = (int)scene;
        SceneManager.LoadScene(sceneNumber, LoadSceneMode.Single);
    }

    public void LoadGameScene()
    {
        LoadScene(Scene.GAME);
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