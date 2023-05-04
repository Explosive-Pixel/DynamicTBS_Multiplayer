using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Networking.Transport;
using System.Linq;

public class Lobby
{
    private LobbyId id;
    public LobbyId Id { get { return id; } }
    public int ShortId { get { return id.Id; } }

    public bool IsActive { get { return connections.Count > 0; } }

    private List<OnlineConnection> connections = new List<OnlineConnection>();
    public List<NetworkConnection> Connections { get { return connections.ConvertAll(cnn => cnn.NetworkConnection); } }
    public List<OnlineConnection> Players { get { return connections.FindAll(cnn => cnn.Role == ClientType.player); } }
    public List<OnlineConnection> Spectators { get { return connections.FindAll(cnn => cnn.Role == ClientType.spectator); } }

    private List<OnlineMessage> messageHistory = new List<OnlineMessage>();
    public List<OnlineMessage> MessageHistory { get { return messageHistory; } }

    public Lobby(LobbyId id, OnlineConnection connection)
    {
        this.id = id;

        AddConnection(connection);
    }

    public bool AddConnection(OnlineConnection connection)
    {
        if(connection.Role == ClientType.player && Players.Count == 2)
        {
            return false;
        }

        connections.Add(connection);

        if(Players.Count == 1)
        {
            connection.IsAdmin = true;
        }

        return true;
    }

    public void CleanUpConnections()
    {
        connections.RemoveAll(cnn => !cnn.NetworkConnection.IsCreated);
    }

    public bool RemoveConnection(NetworkConnection networkConnection)
    {
        OnlineConnection cnn = FindOnlineConnection(networkConnection);

        if(cnn != null)
        {
            if(cnn.IsAdmin)
            {
                SwapAdmin();
            }

            connections.Remove(cnn);
            return true;
        }

        return false;
    }

    public void AssignSides(NetworkConnection networkConnection, PlayerType chosenSide, int boardDesignIndex)
    {
        OnlineConnection cnn = FindOnlineConnection(networkConnection);
        if (cnn != null)
        {
            cnn.Side = chosenSide;

            OnlineConnection other = FindOtherPlayer(cnn);
            if (other != null)
            {
                other.Side = PlayerManager.GetOtherSide(chosenSide);
                OnlineServer.Instance.SendToClient(new MsgUpdateClient
                {
                    isAdmin = other.IsAdmin,
                    side = other.Side.Value,
                    boardDesignIndex = boardDesignIndex
                }, other.NetworkConnection, ShortId);
            }
        }
    }

    // Swaps player which may choose side in next game
    public void SwapAdmin()
    {
        if(Players.Count == 2)
        {
            Players.ForEach(player => player.IsAdmin = !player.IsAdmin);
        }
    }

    public void ArchiveMessage(OnlineMessage msg)
    {
        if (msg.GetType() == typeof(MsgUIAction))
        {
            UIAction uiAction = ((MsgUIAction)msg).uiAction;
            if (uiAction == UIAction.START_GAME)
                messageHistory.Clear();
        }

        if (msg.GetType() != typeof(MsgMetadata))
        {
            messageHistory.Add(msg);
        }
    }

    public OnlineConnection FindOnlineConnection(NetworkConnection networkConnection)
    {
        return connections.Find(cnn => cnn.NetworkConnection == networkConnection);
    }

    private OnlineConnection FindOtherPlayer(OnlineConnection player)
    {
        if(Players.Contains(player))
        {
            return Players.Find(cnn => cnn != player);
        }

        return null;
    }
}
