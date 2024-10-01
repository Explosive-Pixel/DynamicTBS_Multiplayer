using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WSMsgJoinLobby : WSMessage
{
    public string lobbyFullName;
    public ClientInfo clientInfo;
    public bool isReconnect;

    public WSMsgJoinLobby()
    {
        code = WSMessageCode.WSMsgJoinLobbyCode;
        lobbyId = 0;
    }
}
