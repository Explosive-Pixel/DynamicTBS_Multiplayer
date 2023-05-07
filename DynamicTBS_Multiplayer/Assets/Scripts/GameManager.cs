using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameType
{
    LOCAL,
    multiplayer,
    ONLINE
}
public class GameManager : MonoBehaviour
{
    public static GameType gameType = GameType.LOCAL;
    public static GamePhase gamePhase = GamePhase.NONE;

    private void Awake()
    {
        TileSpriteManager.LoadSprites();
        PrefabManager.LoadPrefabs();
    }
    
    public static bool IsHost()
    {
        return gameType == GameType.LOCAL || (OnlineServer.Instance && OnlineServer.Instance.IsActive);
    }

    public static bool IsMultiplayerHost()
    {
        return gameType == GameType.multiplayer && IsHost();
    }

    public static bool IsPlayer()
    {
        return gameType == GameType.LOCAL || (gameType == GameType.ONLINE && OnlineClient.Instance.UserData.Role == ClientType.PLAYER);
    }

    public static void ChangeGamePhase(GamePhase newGamePhase)
    {
        gamePhase = newGamePhase;
        GameEvents.StartGamePhase(gamePhase);
    }
}