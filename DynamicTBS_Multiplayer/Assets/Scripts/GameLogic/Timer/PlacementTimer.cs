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
        //if (GameManager.GameType == GameType.ONLINE && Client.ShouldReadMessage(playerType))
        //    return;

        Debug.Log("Draw no time left consequence for placement.");
        PlacementManager.StartRandomPlacement(playerType);
    }
}
