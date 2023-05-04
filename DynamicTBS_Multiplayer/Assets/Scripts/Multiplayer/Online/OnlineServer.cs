using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using System.Linq;

public class OnlineServer : MonoBehaviour
{
    #region SingletonImplementation
    public static OnlineServer Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    private NetworkDriver driver;

    private List<Lobby> lobbies = new List<Lobby>();

    private int lobbyIdCounter = 0;

    //private List<NetworkConnection> AllConnections { get { return lobbies.ConvertAll(lobby => lobby.Connections).SelectMany(cnn => cnn).ToList(); } }
    private List<NetworkConnection> AllConnections = new List<NetworkConnection>();

    private bool isActive = false;

    private const float KeepAliveTickRate = 20f; // Constant tick rate, so connection won't time out.
    private float lastKeepAlive = 0f; // Timestamp for last connection.

    private Action connectionDropped;

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
            Debug.Log("Server: Currently listening on port " + endPoint.Port);
        }

        isActive = true;
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
    }

    public void SendToClient(OnlineMessage msg, NetworkConnection connection, int lobbyId) // Send specific message to specific client.
    {
        DataStreamWriter writer; // Able to write net messages.
        driver.BeginSend(connection, out writer); // Finds connection and whom to write message to.
        msg.Serialize(ref writer, lobbyId); // Writes message.
        driver.EndSend(writer); // Transmits message.
    }

    public void Broadcast(OnlineMessage msg, int lobbyId)
    {
        Lobby lobby = FindLobby(lobbyId);

        if(lobby != null)
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

    public void CreateLobby(string lobbyName, NetworkConnection cnn, UserData userData)
    {
        OnlineConnection connection = new OnlineConnection(cnn, userData);
        LobbyId lobbyId = new LobbyId(++lobbyIdCounter, lobbyName);
        Lobby lobby = new Lobby(lobbyId, connection);
        lobbies.Add(lobby);

        WelcomeClient(lobby, connection);
    }

    public void JoinLobby(LobbyId lobbyId, NetworkConnection cnn, UserData userData)
    {
        Lobby lobby = FindLobby(lobbyId);

        if(lobby == null)
        {
            // Send message to client that lobby does not exist
            SendToClient(new MsgServerNotification
            {
                serverNotification = ServerNotification.LOBBY_NOT_FOUND
            }, cnn, lobby.ShortId);
        }

        OnlineConnection connection = new OnlineConnection(cnn, userData);
        bool added = lobby.AddConnection(connection);

        if(!added)
        {
            // Send message to client that lobby is full
            SendToClient(new MsgServerNotification
            {
                serverNotification = ServerNotification.CONNECTION_FORBIDDEN_FULL_LOBBY
            }, cnn, lobby.ShortId);
        }

        WelcomeClient(lobby, connection);
    }

    public void SwapAdmin(int lobbyId)
    {
        Lobby lobby = FindLobby(lobbyId);
        lobby.SwapAdmin();
    }

    public void AssignSides(int lobbyId, NetworkConnection cnn, PlayerType chosenSide, int boardDesignIndex)
    {
        Lobby lobby = FindLobby(lobbyId);
        lobby.AssignSides(cnn, chosenSide, boardDesignIndex);
    }
    private Lobby FindLobby(int lobbyId)
    {
        return lobbies.Find(l => l.ShortId == lobbyId);
    }

    private Lobby FindLobby(LobbyId lobbyId)
    {
        return lobbies.Find(l => l.Id.FullId == lobbyId.FullId);
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

        StartCoroutine(SendGameState(connection.NetworkConnection, lobby));
    }

    private IEnumerator SendGameState(NetworkConnection cnn, Lobby lobby)
    {
        if (lobby.MessageHistory.Count > 0)
        {
            Debug.Log("Sending history to client: " + lobby.MessageHistory.Count);
            float delay = 0.1f;

            ToggleSendGameState(cnn, lobby);

            yield return new WaitForSeconds(delay);

            int i = 0;
            while (i < lobby.MessageHistory.Count)
            {
                SendToClient(lobby.MessageHistory[i], cnn, lobby.ShortId);
                i++;
                yield return new WaitForSeconds(delay);
            }

            ToggleSendGameState(cnn, lobby);
        }
    }

    private void ToggleSendGameState(NetworkConnection cnn, Lobby lobby)
    {
        SendToClient(new MsgServerNotification
        {
            serverNotification = ServerNotification.TOGGLE_LOAD_GAME_STATUS
        }, cnn, lobby.ShortId);
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
                        AllConnections.Remove(cnn);
                        lobbies.ForEach(lobby => lobby.RemoveConnection(cnn));
                        connectionDropped?.Invoke();
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
        }
    }

    private void OnDestroy() // Shutting down server on destroy.
    {
        Shutdown();
    }
}
