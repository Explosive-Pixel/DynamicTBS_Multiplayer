using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public enum Scene
{
    MAIN_MENU = 0,
    ONLINE_MENU = 1,
    GAME_SETUP = 2,
    GAME = 3,
    TUTORIAL = 4,
    LORE = 5,
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
        }

        SubscribeEvents();
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