using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WSMsgJoinLobby : WSMessage
{
    public bool create;
    public string lobbyName;
    public string userName;
    public bool isPlayer;

    public WSMsgJoinLobby()
    {
        code = WSMessageCode.WSMsgJoinLobbyCode;
        lobbyId = 0;
    }
}
