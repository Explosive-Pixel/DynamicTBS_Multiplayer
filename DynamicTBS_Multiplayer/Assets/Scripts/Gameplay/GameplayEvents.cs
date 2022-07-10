using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayEvents
{
    public delegate void GameplayPhase();

    public static event GameplayPhase OnGameplayPhaseStart;

    public delegate void FinishAction();
    public static event FinishAction OnFinishMove;

    public static void StartGameplayPhase()
    {
        if (OnGameplayPhaseStart != null)
            OnGameplayPhaseStart();
    }

    public static void MoveFinished()
    {
        if (OnFinishMove != null)
            OnFinishMove();
    }
}