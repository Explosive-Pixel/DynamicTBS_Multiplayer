using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayEvents
{
    public delegate void GameplayPhase();
    public static event GameplayPhase OnGameplayPhaseStart;

    public delegate void FinishAction(UIActionType type);
    public static event FinishAction OnFinishAction;

    public delegate void GameOver(PlayerType winner);
    public static event GameOver OnGameOver;

    public static void StartGameplayPhase()
    {
        if (OnGameplayPhaseStart != null)
            OnGameplayPhaseStart();
    }

    public static void ActionFinished(UIActionType type)
    {
        if (OnFinishAction != null)
            OnFinishAction(type);
    }

    public static void GameIsOver(PlayerType winner)
    {
        if (OnGameOver != null) 
        {
            OnGameOver(winner);
        }
    }
}