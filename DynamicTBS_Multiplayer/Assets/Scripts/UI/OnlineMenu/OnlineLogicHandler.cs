using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineLogicHandler : MonoBehaviour
{
    private readonly List<Scene> dontDestroyOnLoadScenes = new() { Scene.ONLINE_MENU, Scene.GAME };

    private WSMsgLobbyInfo lastWSMsgLobbyInfo = null;

    public WSMsgLobbyInfo LastWSMsgLobbyInfo { get { return lastWSMsgLobbyInfo; } }

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
            MessageReceiver.OnWSMessageReceive += RecordLastMessages;
        }
    }
    #endregion

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (!dontDestroyOnLoadScenes.Contains(SceneChangeManager.Instance.CurrentScene))
            Destroy(gameObject);
    }

    private void RecordLastMessages(WSMessage msg)
    {
        switch (msg.code)
        {
            case WSMessageCode.WSMsgLobbyInfoCode:
                RecordLastLobbyInfoMsg((WSMsgLobbyInfo)msg);
                break;
        }
    }

    private void RecordLastLobbyInfoMsg(WSMsgLobbyInfo msg)
    {
        lastWSMsgLobbyInfo = msg;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        MessageReceiver.OnWSMessageReceive -= RecordLastMessages;
    }
}
