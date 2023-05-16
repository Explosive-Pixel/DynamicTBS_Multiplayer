using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineLogicHandler : MonoBehaviour
{
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
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    #endregion

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (SceneChangeManager.Instance.CurrentScene == Scene.ONLINE_MENU || SceneChangeManager.Instance.CurrentScene == Scene.GAME)
            return;

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
