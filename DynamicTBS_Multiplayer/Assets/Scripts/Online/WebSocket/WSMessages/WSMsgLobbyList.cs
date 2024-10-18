using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using System.Linq;
using UnityEngine;

[Serializable]
public class WSMsgLobbyList : WSMessage
{
    public LobbyInfo[] lobbies;

    public WSMsgLobbyList()
    {
        code = WSMessageCode.WSMsgLobbyListCode;
    }

    public override void HandleMessage()
    {

    }
}
