public enum PlayerActionType
{
    Skip = 0,
    Refresh = 1
}

public interface IPlayerAction
{
    PlayerActionType PlayerActionType { get; }

    bool IsActionAvailable(PlayerType player);

    void ExecuteAction(PlayerType player);
}
