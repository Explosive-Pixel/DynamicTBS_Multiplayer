using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimerConfig
{
    public static float DraftAndPlacementTime { get; set; }
    public static float GameplayTime { get; set; }

    public static void Init(float draftAndPlacementTime, float gameplayTime)
    {
        DraftAndPlacementTime = draftAndPlacementTime;
        GameplayTime = gameplayTime;
    }
}
