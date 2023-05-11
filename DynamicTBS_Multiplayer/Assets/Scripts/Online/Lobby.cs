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
    public LobbyTimer Timer { get { return timer; } }

    private List<OnlineConnection> connections = new List<OnlineConnection>();
    public List<NetworkConnection> Connections { get { return connections.ConvertAll(cnn => cnn.NetworkConnection); } }
    public List<OnlineConnection> Players { get { return connections.FindAll(cnn => cnn.Role == ClientType.PLAYER); } }
    public List<OnlineConnection> Spectators { get { return connections.FindAll(cnn => cnn.Role == ClientType.SPECTATOR); } }

    private List<OnlineMessage> messageHistory = new List<OnlineMessage>();
    public List<OnlineMessage> MessageHistory { get { return messageHistory; } }

    private MapType selectedMap;
    public MapType SelectedMap { get { return selectedMap; } }

    private readonly Dictionary<PlayerType, List<CharacterType>> draft = new Dictionary<PlayerType, List<CharacterType>>();
    public Dictionary<PlayerType, List<CharacterType>> Draft { get { return draft; } }

    private bool gameIsPaused = false;

    private bool inGame = false;
    public bool GameIsRunning { get { return inGame && !gameIsPaused; } }

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

    public void StartGame(TimerSetupType timerSetup, MapType selectedMap)
    {
        draft.Clear();

        inGame = true;
        this.selectedMap = selectedMap;
        timer = new LobbyTimer(timerSetup);

        BroadcastPlayerNames();
    }

    public void UpdateGameInfo(PlayerType currentPlayer, GamePhase gamePhase)
    {
        timer.UpdateGameInfo(currentPlayer, gamePhase);
    }

    public void UpdateTimer()
    {
        if (!GameIsRunning)
            return;

        timer.UpdateTime(ShortId);
    }

    public bool AddConnection(OnlineConnection connection)
    {
        if(connection.Role == ClientType.PLAYER && Players.Count == 2)
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
            connections.Remove(cnn);
            return true;
        }

        return false;
    }

    public void AssignSides(NetworkConnection networkConnection, PlayerType chosenSide)
    {
        OnlineConnection cnn = FindOnlineConnection(networkConnection);
        if (cnn != null)
        {
            cnn.Side = chosenSide;

            OnlineConnection other = FindOtherPlayer(cnn);
            if (other != null)
            {
                other.Side = PlayerManager.GetOtherSide(chosenSide);
                UpdatePlayer(other);
            }
        }
    }

    public void GameOver()
    {
        inGame = false;
        SwapAdmin();
    }

    // Swaps player which may choose side in next game
    private void SwapAdmin()
    {
        if(Players.Count == 2)
        {
            Players.ForEach(player => player.IsAdmin = !player.IsAdmin);
            UpdatePlayers();
        }
    }

    public void UpdateConnectionsAfterReconnect(NetworkConnection networkConnection)
    {
        OnlineConnection cnn = FindOnlineConnection(networkConnection);
        if(cnn != null)
        {
            OnlineConnection other = FindOtherPlayer(cnn);
            if(other != null && other.Side != null)
            {
                cnn.Side = PlayerManager.GetOtherSide(other.Side.Value);
                cnn.IsAdmin = !other.IsAdmin;
                UpdatePlayers();
                BroadcastPlayerNames();
            }
        }
    }

    public void UpdatePlayers()
    {
        Players.ForEach(player => UpdatePlayer(player));
    }

    private void UpdatePlayer(OnlineConnection player)
    {
        string opponentName = "";
        OnlineConnection otherPlayer = FindOtherPlayer(player);
        if(otherPlayer != null)
        {
            opponentName = otherPlayer.Name;
        }

        MsgUpdateClient msg = new MsgUpdateClient
        {
            isAdmin = player.IsAdmin,
            opponentName = opponentName
        };

        if(player.Side != null)
        {
            msg.side = player.Side.Value;
        }

        OnlineServer.Instance.SendToClient(msg, player.NetworkConnection, ShortId);
    }

    private void BroadcastPlayerNames()
    {
        if (Players.Count == 2)
        {
            OnlineServer.Instance.Broadcast(new MsgPlayerNames
            {
                pinkName = Players.Find(p => p.Side == PlayerType.pink).Name,
                blueName = Players.Find(p => p.Side == PlayerType.blue).Name
            }, ShortId);
        }
    }

    public void ArchiveCharacterDraft(PlayerType player, CharacterType character)
    {
        if (!draft.ContainsKey(player))
            draft[player] = new List<CharacterType>();

        draft[player].Add(character);
    }

    public void ArchiveMessage(OnlineMessage msg)
    {
        if (msg.GetType() == typeof(MsgGameOver))
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
