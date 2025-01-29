using System;
using System.Collections.Generic;

[Serializable]
public class LobbyInfo
{
    public string lobbyId;
    public bool isPrivate;
    public LobbyStatus status;
    public GameConfig gameConfig;
    public List<ClientInfo> clients;
}
