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

    private LobbyTimer timer;

    private List<OnlineConnection> connections = new List<OnlineConnection>();
    public List<NetworkConnection> Connections { get { return connections.ConvertAll(cnn => cnn.NetworkConnection); } }
    public List<OnlineConnection> Players { get { return connections.FindAll(cnn => cnn.Role == ClientType.player); } }
    public List<OnlineConnection> Spectators { get { return connections.FindAll(cnn => cnn.Role == ClientType.spectator); } }

    private List<OnlineMessage> messageHistory = new List<OnlineMessage>();
    public List<OnlineMessage> MessageHistory { get { return messageHistory; } }

    private int boardDesignIndex = -1;
    public int BoardDesignIndex { get { return boardDesignIndex; } }

    private bool gameIsPaused = false;
    public bool GameIsPaused { get { return gameIsPaused; } }

    public GamePhase CurrentGamePhase { get { return timer == null ? GamePhase.NONE : timer.CurrentGamePhase; } }

    public Lobby(LobbyId id, OnlineConnection connection)
    {
        this.id = id;

        AddConnection(connection);
    }

    public bool HostsConnection(NetworkConnection cnn)
    {
        return Connections.Contains(cnn);
    }

    public void PauseGame(UIAction uiAction)
    {
        gameIsPaused = uiAction == UIAction.PAUSE_GAME;
    }

    public void InitTimer(float draftAndPlacementTime, float gameplayTime)
    {
        timer = new LobbyTimer(draftAndPlacementTime, gameplayTime);
    }

    public void UpdateGameInfo(PlayerType currentPlayer, GamePhase gamePhase)
    {
        timer.UpdateGameInfo(currentPlayer, gamePhase);
    }

    public void UpdateTimer()
    {
        timer.UpdateTime(ShortId);
    }

    public void SendTimerUpdate()
    {
        timer.BroadcastTimerInfo(ShortId);
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
           /* if(cnn.IsAdmin)
            {
                SwapAdmin();
            } */

            connections.Remove(cnn);
            return true;
        }

        return false;
    }

    public void UpdateConnectionAfterReconnect(NetworkConnection networkConnection)
    {
        OnlineConnection cnn = FindOnlineConnection(networkConnection);
        if (cnn != null)
        {
            OnlineConnection other = FindOtherPlayer(cnn);
            if(other != null && other.Side != null && boardDesignIndex != -1)
            {
                cnn.Side = PlayerManager.GetOtherSide(other.Side.Value);
                cnn.IsAdmin = !other.IsAdmin;
                OnlineServer.Instance.SendToClient(new MsgUpdateClient
                {
                    isAdmin = cnn.IsAdmin,
                    side = cnn.Side.Value,
                    boardDesignIndex = boardDesignIndex
                }, networkConnection, ShortId);
            }
        }
    }

    public void AssignSides(NetworkConnection networkConnection, PlayerType chosenSide, int boardDesignIndex)
    {
        OnlineConnection cnn = FindOnlineConnection(networkConnection);
        if (cnn != null)
        {
            cnn.Side = chosenSide;
            this.boardDesignIndex = boardDesignIndex;

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
            UpdateAdmins();
        }
    }

    private void UpdateAdmins()
    {
        Players.ForEach(player => OnlineServer.Instance.SendToClient(new MsgUpdateClient
        {
            isAdmin = player.IsAdmin,
            side = player.Side.Value,
            boardDesignIndex = boardDesignIndex
        }, player.NetworkConnection, ShortId));
    }

    public void ArchiveMessage(OnlineMessage msg)
    {
        if (msg.GetType() == typeof(MsgStartGame))
        {
            messageHistory.Clear();
        }

        if (msg.GetType() != typeof(MsgMetadata) && msg.GetType() != typeof(MsgSyncTimer))
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
