using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMetadata
{
    public PlayerType ExecutingPlayer { get; set; }
    public ActionType ExecutedActionType { get; set; }
    private int actionCount = 1;
    public int ActionCount { get { return actionCount; } set { actionCount = value; } }
    public CharacterMB CharacterInAction { get; set; }
    public Vector3? CharacterInitialPosition { get; set; }
    public Vector3? ActionDestinationPosition { get; set; }
}
