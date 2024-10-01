using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayConfig
{
    public static GameConfig GameConfig
    {
        get
        {
            return new GameConfig()
            {
                timerConfig = new GameConfig.TimerConfig()
                {
                    draftAndPlacementTime = TimerConfig.DraftAndPlacementTime,
                    gameplayTime = TimerConfig.GameplayTime,
                    gameplayTimePerTurn = true
                },
                mapType = Board.selectedMapType
            };
        }
    }

    public static void Init(GameConfig gameConfig)
    {
        TimerConfig.Init(gameConfig.timerConfig.draftAndPlacementTime, gameConfig.timerConfig.gameplayTime);
        Board.selectedMapType = gameConfig.mapType;
    }
}
