using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby
{
    public LobbyId LobbyId { get; private set; }
    public List<ClientInfo> Clients { get; private set; }

    public Lobby(LobbyInfo lobbyInfo)
    {
        LobbyId = LobbyId.FromFullId(lobbyInfo.lobbyId);
        GameplayConfig.Init(lobbyInfo.gameConfig);
        Clients = lobbyInfo.clients;
    }

    public List<ClientInfo> Players { get { return Clients.FindAll(client => client.isPlayer); } }
    public List<ClientInfo> Spectators { get { return Clients.FindAll(client => !client.isPlayer); } }

    public int PlayerCount { get { return Players.Count; } }
    public int SpectatorCount { get { return Spectators.Count; } }

    public ClientInfo GetClientInfo(string uuid)
    {
        return Clients.Find(client => client.uuid == uuid);
    }

    public ClientInfo GetPlayer(PlayerType side)
    {
        return Players.Find(player => player.side == side);
    }

    public string GetPlayerName(PlayerType side)
    {
        ClientInfo player = GetPlayer(side);
        if (player != null)
            return GetPlayer(side).name;

        return side == PlayerType.pink ? "Pink" : "Blue";
    }
}
