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
                originalDuration = GameSetup.TimerSetup.DraftAndPlacementTime;
                break;
            case GamePhase.PLACEMENT:
                timerScript = gameObject.AddComponent<PlacementTimer>();
                originalDuration = GameSetup.TimerSetup.DraftAndPlacementTime;
                break;
            case GamePhase.GAMEPLAY:
                timerScript = gameObject.AddComponent<GameplayTimer>();
                originalDuration = GameSetup.TimerSetup.GameplayTime;
                break;
        }

        if (timerScript != null)
        {
            timerScript.Init(gamePhase, playerType, originalDuration);
        }

        return timerScript;
    }
}
