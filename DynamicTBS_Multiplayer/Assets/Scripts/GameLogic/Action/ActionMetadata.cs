using UnityEngine;

public class ActionMetadata
{
    public PlayerType ExecutingPlayer { get; set; }
    public ActionType ExecutedActionType { get; set; }
    public Character CharacterInAction { get; set; }
    public Vector3? CharacterInitialPosition { get; set; }
    public Vector3? ActionDestinationPosition { get; set; }
}
