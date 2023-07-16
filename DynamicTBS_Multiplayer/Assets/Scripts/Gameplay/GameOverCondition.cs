using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    MASTER_TOOK_CONTROL,
    MASTER_DIED,
    PLAYER_SURRENDERED,
    NO_AVAILABLE_ACTION,
    DRAW_ACCEPTED,
    PLAYER_TIMEOUT
}

static class GameOverConditionMethods
{
    public static string ToText(this GameOverCondition condition, PlayerType? winner)
    {
        switch (condition)
        {
            case GameOverCondition.MASTER_TOOK_CONTROL:
                return "The " + winner + " " + CharacterType.MasterChar.Name() + " took control over the engine.";
            case GameOverCondition.MASTER_DIED:
                return "The " + PlayerManager.GetOtherSide(winner.Value) + " " + CharacterType.MasterChar.Name() + " died.";
            case GameOverCondition.PLAYER_SURRENDERED:
                return "Player " + PlayerManager.GetOtherSide(winner.Value) + " surrendered.";
            case GameOverCondition.NO_AVAILABLE_ACTION:
                return "Player " + PlayerManager.GetOtherSide(winner.Value) + " has no available actions.";
            case GameOverCondition.DRAW_ACCEPTED:
                return "The players have agreed on a draw.";
            case GameOverCondition.PLAYER_TIMEOUT:
                return "Player " + PlayerManager.GetOtherSide(winner.Value) + " took too much time.";
            default:
                return "";
        }
    }
}
