using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public static class GameEvents
{
    public delegate void GameStart();
    public static event GameStart OnGameStart;

    public delegate void ChangeGamePhase(GamePhase gamePhase);
    public static event ChangeGamePhase OnGamePhaseStart;
    public static event ChangeGamePhase OnGamePhaseEnd;

    public delegate void LoadGame(bool isLoading);
    public static event LoadGame OnGameIsLoading;

    public delegate void BootGame();
    public static event BootGame OnGameBoot;

    public static void StartGame()
    {
        if (OnGameStart != null)
            OnGameStart();
    }

    public static void StartGamePhase(GamePhase gamePhase)
    {
        if (OnGamePhaseStart != null)
            OnGamePhaseStart(gamePhase);
    }

    public static void EndGamePhase(GamePhase gamePhase)
    {
        if (OnGamePhaseEnd != null)
            OnGamePhaseEnd(gamePhase);
    }

    public static void IsGameLoading(bool isLoading)
    {
        if (OnGameIsLoading != null)
            OnGameIsLoading(isLoading);
    }

    public static void GameHasBooted()
    {
        if (OnGameBoot != null)
            OnGameBoot();
    }
}
