public class PlayerActionUtils
{
    public static bool HasAvailablePlayerAction(PlayerType player)
    {
        foreach (IPlayerAction action in PlayerActionRegistry.GetActions())
        {
            if (action.IsActionAvailable(player))
                return true;
        }

        return false;
    }

    public static void ExecuteAction(IPlayerAction playerAction, PlayerType player)
    {
        Action action = new()
        {
            ExecutingPlayer = player,
            PlayerActionType = playerAction.PlayerActionType
        };

        if (GameManager.GameType == GameType.LOCAL)
        {
            FinishAction(action);
            return;
        }

        WSMsgPerformAction.SendPerformActionMessage(action);
    }

    public static void ExecuteAction(Action action)
    {
        FinishAction(action);
    }

    private static void FinishAction(Action action)
    {
        if (action.PlayerActionType == null)
            return;

        IPlayerAction playerAction = PlayerActionRegistry.GetAction(action.PlayerActionType.Value);
        playerAction.ExecuteAction(action.ExecutingPlayer);
        GameplayEvents.ActionFinished(action);
    }
}
