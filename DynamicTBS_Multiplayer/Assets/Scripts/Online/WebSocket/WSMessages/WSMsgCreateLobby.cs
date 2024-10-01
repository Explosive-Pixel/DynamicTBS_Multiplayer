using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSMsgCreateLobby : WSMessage
{
    public string lobbyName;
    public bool isPrivateLobby;
    public ClientInfo clientInfo;
    public GameConfig gameConfig;

    public WSMsgCreateLobby()
    {
        code = WSMessageCode.WSMsgCreateLobbyCode;
    }
}
