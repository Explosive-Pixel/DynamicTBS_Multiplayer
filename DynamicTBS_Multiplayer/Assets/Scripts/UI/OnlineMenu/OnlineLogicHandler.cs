using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineLogicHandler : MonoBehaviour
{
    private readonly List<Scene> dontDestroyOnLoadScenes = new() { Scene.ONLINE_MENU, Scene.GAME };

    #region SingletonImplementation
    public static OnlineLogicHandler Instance { set; get; }

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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }
    #endregion

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (dontDestroyOnLoadScenes.Contains(SceneChangeManager.Instance.CurrentScene))
            return;

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
