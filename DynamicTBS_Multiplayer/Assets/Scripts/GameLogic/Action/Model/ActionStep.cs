using UnityEngine;

public class ActionStep
{
    public ActionType ActionType { get; set; }
    public Character CharacterInAction { get; set; }
    public Vector3? CharacterInitialPosition { get; set; }
    public Vector3? ActionDestinationPosition { get; set; }
    public bool ActionFinished { get; set; } // Indicates whether this ActionStep is the last step of the action
}
