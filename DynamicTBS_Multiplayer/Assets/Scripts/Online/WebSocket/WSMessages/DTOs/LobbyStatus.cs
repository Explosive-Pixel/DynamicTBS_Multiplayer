using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum LobbyStatus
{
    [Description("Under construction")]
    UNDER_CONSTRUCTION,
    [Description("Waiting for player")]
    WAITING_FOR_PLAYER,
    [Description("In game")]
    IN_GAME
}
