using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimerConfig
{
    private static float draftAndPlacementTime;
    public static float DraftAndPlacementTime => draftAndPlacementTime;

    private static float gameplayTime;
    public static float GameplayTime => gameplayTime;

    public static void Init(float draftAndPlacementTime, float gameplayTime)
    {
        TimerConfig.draftAndPlacementTime = draftAndPlacementTime;
        TimerConfig.gameplayTime = gameplayTime;
    }
}
