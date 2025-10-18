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

    public static void ExecuteAction(IPlayerAction action, PlayerType player)
    {
        action.ExecuteAction(player);

        GameplayEvents.ActionFinished(new ActionMetadata
        {
            ExecutingPlayer = player,
            ExecutedActionType = ActionType.PlayerAction,
            PlayerActionType = action.PlayerActionType
        });
    }
}
