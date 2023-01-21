using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents 
{
    public delegate void GamePhase();
    public static event GamePhase OnGameStart;

    public delegate void LoadGame(bool isLoading);
    public static event LoadGame OnGameIsLoading;

    public static void StartGame()
    {
        if (OnGameStart != null)
            OnGameStart();
    }

    public static void IsGameLoading(bool isLoading)
    {
        if (OnGameIsLoading != null)
            OnGameIsLoading(isLoading);
    }
}
