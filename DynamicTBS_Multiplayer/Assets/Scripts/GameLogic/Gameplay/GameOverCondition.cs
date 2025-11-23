// Implement the following win/loss/draw-conditions...
// (DONE) WIN/LOSS: A side wins, if...
//           - the master unit is first to activate its active ability on the goal square.
//           - the opposing master unit is killed while the own master unit is still alive.
//           - the opposing side has no more legal moves, but 2 actions in their turn.
// (TODO) DRAW: A game is drawn, if...
//           - no mechanic units are alive on either side...
//           & the goal square can't be reached...
//           & both master units can't be killed.
public enum GameOverCondition
{
    CAPTAIN_TOOK_CONTROL = 0,
    CAPTAIN_DIED = 1,
    PLAYER_SURRENDERED = 2,
    NO_AVAILABLE_ACTION = 3,
    DRAW_ACCEPTED = 4,
    PLAYER_TIMEOUT = 5
}

static class GameOverConditionMethods
{
    public static string ToText(this GameOverCondition condition, PlayerType? winner)
    {
        if (winner == PlayerType.none)
            return "";

        return condition switch
        {
            GameOverCondition.CAPTAIN_TOOK_CONTROL => winner == PlayerType.blue ? "gameOverCondition_BlueCaptainTookControl" : "gameOverCondition_PinkCaptainTookControl",
            GameOverCondition.CAPTAIN_DIED => winner == PlayerType.blue ? "gameOverCondition_PinkCaptainDied" : "gameOverCondition_BlueCaptainDied",
            GameOverCondition.PLAYER_SURRENDERED => winner == PlayerType.blue ? "gameOverCondition_PlayerPinkSurrendered" : "gameOverCondition_PlayerBlueSurrendered",
            GameOverCondition.NO_AVAILABLE_ACTION => winner == PlayerType.blue ? "gameOverCondition_PinkHasNoAvailableActions" : "gameOverCondition_BlueHasNoAvailableActions",
            GameOverCondition.PLAYER_TIMEOUT => winner == PlayerType.blue ? "gameOverCondition_PinkTookTooMuchTime" : "gameOverCondition_BlueTookTooMuchTime",
            _ => "",
        };
    }
}
