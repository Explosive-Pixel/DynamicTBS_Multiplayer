using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSetup
{
    public float DraftAndPlacementTime { get; private set; }
    public float GameplayTime { get; private set; }

    public string DraftAndPlacementTimeFormatted { get { return TimerUtils.FormatTime(DraftAndPlacementTime); } }
    public string GameplayTimeFormatted { get { return TimerUtils.FormatTime(GameplayTime); } }

    public GameConfig.TimerConfig TimerConfig
    {
        get
        {
            return
                new GameConfig.TimerConfig()
                {
                    draftAndPlacementTime = DraftAndPlacementTime,
                    gameplayTime = GameplayTime,
                    gameplayTimePerTurn = true
                };
        }
    }

    public TimerSetup(float draftAndPlacementTime, float gameplayTime)
    {
        DraftAndPlacementTime = draftAndPlacementTime;
        GameplayTime = gameplayTime;
    }

    public TimerSetup(GameConfig.TimerConfig timerConfig)
    {
        DraftAndPlacementTime = timerConfig.draftAndPlacementTime;
        GameplayTime = timerConfig.gameplayTime;
    }
}
