using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSMsgLobbyInfo : WSMessage
{
    public LobbyInfo lobbyInfo;

    public WSMsgLobbyInfo()
    {
        code = WSMessageCode.WSMsgLobbyInfoCode;
    }

    public override void HandleMessage()
    {
        Client.UpdateLobby(lobbyInfo);
    }
}
