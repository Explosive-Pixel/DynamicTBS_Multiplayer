using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftTimer : GameTimer
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
        if (GameManager.GameType == GameType.ONLINE && (Client.Side != playerType || Client.IsLoadingGame))
            return;

        DraftManager.RandomDrafts(playerType);
    }
}
