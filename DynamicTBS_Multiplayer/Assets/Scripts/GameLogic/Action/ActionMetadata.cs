using UnityEngine;

public class ActionMetadata
{
    public PlayerType ExecutingPlayer { get; set; }
    public ActionType ExecutedActionType { get; set; }
    public PlayerActionType? PlayerActionType { get; set; }
    public Character CharacterInAction { get; set; }
    public Vector3? CharacterInitialPosition { get; set; }
    public Vector3? ActionDestinationPosition { get; set; }
    public Vector3? SecondActionDestinationPosition { get; set; }
}
