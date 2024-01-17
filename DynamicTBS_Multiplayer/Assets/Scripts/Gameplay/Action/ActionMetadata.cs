using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMetadata
{
    public PlayerType ExecutingPlayer { get; set; }
    public ActionType ExecutedActionType { get; set; }
    public Character CharacterInAction { get; set; }
    public Vector3? CharacterInitialPosition { get; set; }
    // public Tile CharacterInitialTile { get; set; }
    // public Tile ActionDestinationTile { get; set; }
    public Vector3? ActionDestinationPosition { get; set; }
}
