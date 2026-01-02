using System;
using UnityEngine;

public class PlacementTimer : GameTimer
{
    public override void SetActive(DateTime startTime)
    {
        if (!isRunning)
            StartTimer(startTime, originalDuration);
        else
            UnpauseTimer(startTime);
    }

    public override void SetInactive()
    {
        PauseTimer();
    }

    public override void DrawNoTimeLeftConsequences()
    {
        PlacementManager.StartRandomPlacement(playerType);
    }
}
