using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using System.Linq;
using System.Net;

public class OnlineServer : MonoBehaviour
{
    #region SingletonImplementation
    public static OnlineServer Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    [SerializeField] private MessageBroker messageBroker;

    private NetworkDriver driver;

    private List<Lobby> lobbies = new List<Lobby>();
    public int LobbyCount { get { return lobbies.Count; } }

    private int lobbyIdCounter = 0;

    private List<NetworkConnection> AllConnections = new List<NetworkConnection>();
    public int ConnectionCount { get { return AllConnections.Count; } }

    private bool isActive = false;
    public bool IsActive { get { return isActive; } }

    private const float KeepAliveTickRate = 20f; // Constant tick rate, so connection won't time out.
    private float lastKeepAlive = 0f; // Timestamp for last connection.

    private Action connectionDropped;

    private string ip;
    public string IP { get { return ip; } }

    #region Networking

    public void Init(ushort port) // Initiation method.
    {
        driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4; // Point where clients connect to server.
        endPoint.Port = port; // Port for this server.

        if (driver.Bind(endPoint) != 0)
        {
            Debug.Log("Server: Unable to bind to port " + endPoint.Port);
            return;
        }
        else
        {
            driver.Listen(); // Makes server listen to clients.
            Debug.Log("Server: Currently listening to port " + endPoint.Port);

            try
            {
                using WebClient client = new WebClient();
                ip = client.DownloadString("https://api.ipify.org");
            }
            catch (Exception)
            {
                ip = "Unable to find out IP address.";
            }
        }

        messageBroker.Driver = driver;

        isActive = true;
        SendNotification("Server is active!");
    }

    private void Update()
    {
        if (!isActive) return;

        driver.ScheduleFlushSend(default).Complete();

        KeepAlive(); // Prevents connection timeout.

        driver.ScheduleUpdate().Complete(); // Makes sure driver processed all incoming messages.

        CleanUpConnections(); // Cleans up connections of disconnected clients.
        AcceptNewConnections(); // Accepts new connections

        UpdateMessagePump(); // Check for messages and if server has to reply.

        lobbies.ForEach(lobby => lobby.UpdateTimer()); // Update timer of all lobbies.
    }

    public void SendToClient(OnlineMessage msg, NetworkConnection connection, int lobbyId) // Send specific message to specific client.
    {
        messageBroker.SendMessage(msg, connection, lobbyId);
    }

    public void Broadcast(OnlineMessage msg, int lobbyId)
    {
        Lobby lobby = FindLobby(lobbyId);

        if (lobby != null)
        {
            Broadcast(msg, lobby);
        }
    }

    public void Broadcast(OnlineMessage msg, Lobby lobby) // Send message to every client.
    {
        lobby.ArchiveMessage(msg);
        for (int i = 0; i < lobby.Connections.Count; i++)
        {
            NetworkConnection cnn = lobby.Connections[i];
            if (cnn.IsCreated)
            {
                SendToClient(msg, cnn, lobby.ShortId);
            }
        }
    }

    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection)) // Checks if a client tries to connect who's not the default connection.
        {
            Debug.Log("New client connected: " + c.ToString());
            AllConnections.Add(c);
        }
    }

    private void UpdateMessagePump()
    {
        try
        {
            DataStreamReader stream; // Reads incoming messages.
            for (int i = 0; i < AllConnections.Count; i++)
            {
                NetworkConnection cnn = AllConnections[i];
                NetworkEvent.Type cmd;
                // There are 4 types of network events:
                // Empty = nothing was sent.
                // Connect = connection is made.
                // Data = any net-message sent.
                // Disconnect = connection is severed.

                while ((cmd = driver.PopEventForConnection(cnn, out stream)) != NetworkEvent.Type.Empty)
                {
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        OnlineMessageHandler.HandleData(stream, cnn, this);
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        Debug.Log("Client disconnected from server: " + cnn.ToString());
                        DisconnectClient(cnn);
                    }
                }
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    private void KeepAlive()
    {
        if (Time.time - lastKeepAlive > KeepAliveTickRate)
        {
            lastKeepAlive = Time.time;
            lobbies.ForEach(lobby => BroadcastMetadata(lobby));
        }
    }

    private void BroadcastMetadata(Lobby lobby)
    {
        Broadcast(new MsgMetadata
        {
            playerCount = lobby.Players.Count,
            spectatorCount = lobby.Spectators.Count
        }, lobby);
    }

    private void DisconnectClient(NetworkConnection cnn)
    {
        AllConnections.Remove(cnn);
        messageBroker.RemoveConnection(cnn);

        Lobby lobby = FindLobby(cnn);
        if (lobby != null)
        {
            lobby.RemoveConnection(cnn);
            BroadcastMetadata(lobby);
        }

        connectionDropped?.Invoke();
        NotifyMetadata("Client disconnected from server.");
    }

    private void CleanUpConnections()
    {
        lobbies.ForEach(lobby => lobby.CleanUpConnections());
        CleanUpLobbies();
    }

    private void CleanUpLobbies()
    {
        lobbies.RemoveAll(lobby => !lobby.IsActive);
    }

    public void Shutdown() // For shutting down the server.
    {
        if (isActive)
        {
            Debug.Log("Shutting down Server.");
            driver.Dispose();
            lobbies.Clear();
            isActive = false;
            SendNotification("Server shut down!");
        }
    }

    #endregion

    public void AddClient(string lobbyName, int LobbyId, NetworkConnection cnn, UserData userData, bool newLobby)
    {
        if (newLobby)
        {
            CreateLobby(lobbyName, cnn, userData);
        }
        else
        {
            JoinLobby(new LobbyId(LobbyId, lobbyName), cnn, userData);
        }
        NotifyMetadata("New Client incoming!");
    }

    private void CreateLobby(string lobbyName, NetworkConnection cnn, UserData userData)
    {
        OnlineConnection connection = new OnlineConnection(cnn, userData);
        LobbyId lobbyId = new LobbyId(++lobbyIdCounter, lobbyName);
        Lobby lobby = new Lobby(lobbyId, connection);
        lobbies.Add(lobby);

        WelcomeClient(lobby, connection);
    }

    private void JoinLobby(LobbyId lobbyId, NetworkConnection cnn, UserData userData)
    {
        Lobby lobby = FindLobby(lobbyId);

        if (lobby == null)
        {
            // Send message to client that lobby does not exist
            SendToClient(new MsgServerNotification
            {
                serverNotification = ServerNotification.LOBBY_NOT_FOUND
            }, cnn, 0);
            DisconnectClient(cnn);
            return;
        }

        OnlineConnection connection = new OnlineConnection(cnn, userData);
        bool added = lobby.AddConnection(connection);

        if (!added)
        {
            // Send message to client that lobby is full
            SendToClient(new MsgServerNotification
            {
                serverNotification = ServerNotification.CONNECTION_FORBIDDEN_FULL_LOBBY
            }, cnn, lobby.ShortId);
            DisconnectClient(cnn);
            return;
        }

        WelcomeClient(lobby, connection);
    }

    public void GameOver(int lobbyId)
    {
        Lobby lobby = FindLobby(lobbyId);
        lobby.GameOver();
    }

    public void AssignSides(int lobbyId, NetworkConnection cnn, PlayerType chosenSide)
    {
        Lobby lobby = FindLobby(lobbyId);
        lobby.AssignSides(cnn, chosenSide);
    }

    public void UpdateGameInfo(int lobbyId, PlayerType currentPlayer, GamePhase gamePhase)
    {
        Lobby lobby = FindLobby(lobbyId);
        lobby.UpdateGameInfo(currentPlayer, gamePhase);
    }

    public void PauseGame(int lobbyId, UIAction uiAction)
    {
        Lobby lobby = FindLobby(lobbyId);
        lobby.PauseGame(uiAction);
    }

    private void WelcomeClient(Lobby lobby, OnlineConnection connection)
    {
        MsgWelcomeClient msg = new MsgWelcomeClient
        {
            lobbyName = lobby.Id.Name,
            isAdmin = connection.IsAdmin
        };

        SendToClient(msg, connection.NetworkConnection, lobby.ShortId);

        BroadcastMetadata(lobby);

        if (connection.Role == ClientType.PLAYER)
            lobby.UpdatePlayers();

        lobby.SendGameState(connection.NetworkConnection);
    }

    public void StartGame(int lobbyId, TimerSetupType timerSetup, MapType selectedMap)
    {
        Lobby lobby = FindLobby(lobbyId);
        lobby.StartGame(timerSetup, selectedMap);
    }

    public void ArchiveCharacterDraft(MsgDraftCharacter msg)
    {
        Lobby lobby = FindLobby(msg.LobbyId);
        lobby.ArchiveCharacterDraft(msg.playerId, msg.characterType);
    }

    public Lobby FindLobby(int lobbyId)
    {
        return lobbies.Find(l => l.ShortId == lobbyId);
    }

    private Lobby FindLobby(LobbyId lobbyId)
    {
        return lobbies.Find(l => l.Id.FullId == lobbyId.FullId);
    }

    private Lobby FindLobby(NetworkConnection cnn)
    {
        return lobbies.Find(l => l.HostsConnection(cnn));
    }

    private void NotifyMetadata(string msg)
    {
        SendNotification(msg + "\nConnected Clients: " + ConnectionCount + "\nActive Lobbies: " + LobbyCount);
    }

    private void SendNotification(string msg)
    {
        Telegram.SendMessage(msg);
    }

    private void OnDestroy() // Shutting down server on destroy.
    {
        Shutdown();
    }
}
