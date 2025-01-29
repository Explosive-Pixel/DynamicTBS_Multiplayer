using System.ComponentModel;

public enum LobbyStatus
{
    [Description("Under construction")]
    UNDER_CONSTRUCTION,
    [Description("Waiting for player")]
    WAITING_FOR_PLAYER,
    [Description("In game")]
    IN_GAME
}
