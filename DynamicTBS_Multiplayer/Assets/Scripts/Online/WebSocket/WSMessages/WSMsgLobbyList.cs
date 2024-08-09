using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using System.Linq;
using UnityEngine;

[Serializable]
public class WSMsgLobbyList : WSMessage
{
    [Serializable]
    public class LobbyMetadata
    {
        public string lobbyName;
        public int playerCount;
        public int spectatorCount;
        public float draftAndPlacementTimeInSeconds;
        public float gameplayTimeInSeconds;
        public MapType mapType;
    }

    public LobbyMetadata[] lobbies;

    public WSMsgLobbyList()
    {
        code = WSMessageCode.WSMsgLobbyListCode;
    }

    public override void HandleMessage()
    {

    }
}
