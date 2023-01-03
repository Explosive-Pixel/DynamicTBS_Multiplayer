using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents 
{
    public delegate void GamePhase();
    public static event GamePhase OnGameStart;

    public static void StartGame()
    {
        if (OnGameStart != null)
            OnGameStart();
    }
}
