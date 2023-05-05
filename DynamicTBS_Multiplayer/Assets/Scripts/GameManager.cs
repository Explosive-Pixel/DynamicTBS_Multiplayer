using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameType gameType = GameType.local;

    private void Awake()
    {
        TileSpriteManager.LoadSprites();
        PrefabManager.LoadPrefabs();
    }
    
    public static bool IsHost()
    {
        return gameType == GameType.local || (Server.Instance && Server.Instance.IsActive);
    }

    public static bool IsMultiplayerHost()
    {
        return gameType == GameType.multiplayer && IsHost();
    }

    public static bool IsPlayer()
    {
        return gameType == GameType.local || (gameType == GameType.online && OnlineClient.Instance.UserData.Role == ClientType.player);
    }
}