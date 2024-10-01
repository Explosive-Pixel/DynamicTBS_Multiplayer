using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LobbyInfo
{
    public string lobbyId;
    public GameConfig gameConfig;
    public List<ClientInfo> clients;
}
