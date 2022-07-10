using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayEvents
{
    public delegate void GameplayPhase();

    public static event GameplayPhase OnGameplayPhaseStart;

    public static void StartGameplayPhase()
    {
        if (OnGameplayPhaseStart == null)
            OnGameplayPhaseStart();
    }
}