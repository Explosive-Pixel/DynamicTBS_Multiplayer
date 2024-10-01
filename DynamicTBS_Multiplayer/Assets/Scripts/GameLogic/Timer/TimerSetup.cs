using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSetup
{
    public float DraftAndPlacementTime { get; private set; }
    public float GameplayTime { get; private set; }

    public string DraftAndPlacementTimeFormatted { get { return TimerUtils.FormatTime(DraftAndPlacementTime); } }
    public string GameplayTimeFormatted { get { return TimerUtils.FormatTime(GameplayTime); } }

    public TimerSetup(float draftAndPlacementTime, float gameplayTime)
    {
        DraftAndPlacementTime = draftAndPlacementTime;
        GameplayTime = gameplayTime;
    }
}
