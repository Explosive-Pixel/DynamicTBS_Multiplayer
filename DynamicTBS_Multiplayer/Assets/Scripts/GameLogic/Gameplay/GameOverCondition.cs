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
