using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimerFactory
{
    public static GameTimer Create(GameObject gameObject, GamePhase gamePhase, PlayerType playerType)
    {
        GameTimer timerScript = null;
        float originalDuration = 0;

        switch (gamePhase)
        {
            case GamePhase.DRAFT:
                timerScript = gameObject.AddComponent<DraftTimer>();
                originalDuration = TimerConfig.DraftAndPlacementTime;
                break;
            case GamePhase.PLACEMENT:
                timerScript = gameObject.AddComponent<PlacementTimer>();
                originalDuration = TimerConfig.DraftAndPlacementTime;
                break;
            case GamePhase.GAMEPLAY:
                timerScript = gameObject.AddComponent<GameplayTimer>();
                originalDuration = TimerConfig.GameplayTime;
                break;
        }

        if (timerScript != null)
        {
            timerScript.Init(gamePhase, playerType, originalDuration);
        }

        return timerScript;
    }
}
