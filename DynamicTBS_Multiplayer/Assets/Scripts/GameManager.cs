using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameType
{
    LOCAL,
    ONLINE
}

public enum GamePhase
{
    DRAFT,
    PLACEMENT,
    GAMEPLAY,
    NONE
}

public class GameManager : MonoBehaviour
{
    public static GameType gameType = GameType.LOCAL;
    public static GamePhase gamePhase = GamePhase.NONE;

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