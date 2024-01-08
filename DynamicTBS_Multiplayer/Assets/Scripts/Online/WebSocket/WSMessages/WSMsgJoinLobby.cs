using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WSMsgJoinLobby : WSMessage
{
    public bool create;
    public string lobbyName;
    public string clientUUID;
    public string userName;
    public bool isPlayer;
    public bool isReconnect;

    public WSMsgJoinLobby()
    {
        code = WSMessageCode.WSMsgJoinLobbyCode;
        lobbyId = 0;
    }
}
