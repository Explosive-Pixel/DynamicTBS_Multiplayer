using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Networking.Transport;
using System.Linq;

public class Server : MonoBehaviour
{
    #region SingletonImplementation

    public static Server Instance { set; get; }
    private GameObject onlineGameManagerObject;

    private void Awake()
    {
        Instance = this;
        Shutdown();
        onlineGameManagerObject = this.gameObject;
        DontDestroyOnLoad(onlineGameManagerObject);
    }

    #endregion

    class PlayerInfo
    {
        public PlayerType? side = null;
        public bool IsAdmin = false;
    }

    public bool IsActive { get { return isActive;  } }
    public int PlayerCount { get { return isActive ? players.Length : 0; } }

    private NetworkDriver driver;
    private NativeList<NetworkConnection> players;
    private readonly List<NetworkConnection> spectators = new List<NetworkConnection>();
    private readonly List<NetworkConnection> allConnections = new List<NetworkConnection>();
    private readonly List<NetworkConnection> nonAssignedConnections = new List<NetworkConnection>();

    private readonly Dictionary<NetworkConnection, PlayerInfo> playerInfo = new Dictionary<NetworkConnection, PlayerInfo>();

    private readonly List<NetMessage> messageHistory = new List<NetMessage>();

    private bool isActive = false;
    private const float KeepAliveTickRate = 20f; // Constant tick rate, so connection won't time out.
    private float lastKeepAlive = 0f; // Timestamp for last connection.

    private Action connectionDropped;

    private void Update()
    {
        if (!isActive) return;

        driver.ScheduleFlushSend(default).Complete();

        KeepAlive(); // Prevents connection timeout.

        driver.ScheduleUpdate().Complete(); // Makes sure driver processed all incoming messages.

        CleanUpConnections(); // Cleans up connections of disconnected clients.
        AcceptNewConnections(); // Accepts new connections if capacity is available.

        UpdateMessagePump(); // Check for messages and if server has to reply.
    }

    #region CommonNetCode

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

        players = new NativeList<NetworkConnection>(2, Allocator.Persistent); // Allows up to two connections at a time.
        isActive = true;
        SubscribeEvents();
    }


    public bool RegisterClient(NetworkConnection c, ClientType role)
    {
        if(nonAssignedConnections.Contains(c))
        {
            if(role == ClientType.player)
            {
                if(PlayerCount == 2)
                {
                    SendToClient(new NetConnectionForbidden(), c);
                    DropConnection(c);
                    return false;
                }
                RegisterPlayer(c);
            } else
            {
                spectators.Add(c);
            }
            BroadcastMetadata();
            nonAssignedConnections.Remove(c);
            UpdateConnections();

            return true;
        }

        if(allConnections.Contains(c))
        {
            return true;
        }

        return false;
    }

    public void ReassignSides(NetworkConnection cnn, PlayerType side)
    {
        foreach (NetworkConnection c in playerInfo.Keys)
        {
            if (c == cnn)
            {
                playerInfo[c].side = side;
            }
            else
            {
                playerInfo[c].side = GetOtherSide(side);
            }
        }
    }

    public void SwapAdmin()
    {
        foreach (NetworkConnection c in playerInfo.Keys)
        {
            playerInfo[c].IsAdmin = !playerInfo[c].IsAdmin;
        }
    }

    private void RegisterPlayer(NetworkConnection c)
    {
        PlayerInfo playerMetadata = new PlayerInfo();

        if(PlayerCount == 0)
        {
            playerMetadata.IsAdmin = true;
        }

        if(PlayerCount == 1)
        {
            PlayerInfo other = playerInfo[playerInfo.Keys.First()];
            playerMetadata.IsAdmin = !other.IsAdmin;
            playerMetadata.side = GetOtherSide(other.side);
        }

        players.Add(c);
        playerInfo.Add(c, playerMetadata);
    }

    private bool IsAdmin(NetworkConnection cnn)
    {
        return playerInfo[cnn].IsAdmin;
    }

    private PlayerType? GetSide(NetworkConnection cnn)
    {
        return playerInfo[cnn].side;
    }

    private int GetSideAsInt(NetworkConnection cnn)
    {
        PlayerType? side = GetSide(cnn);
        return side != null ? (int)side : 0;
    }

    private PlayerType? GetOtherSide(PlayerType? side)
    {
        if(side == null)
        {
            return null;
        }

        return PlayerManager.GetOtherSide(side.Value);
    }

    public void Shutdown() // For shutting down the server.
    {
        if (isActive)
        {
            Debug.Log("Shutting down Server.");
            driver.Dispose();
            players.Dispose();
            spectators.Clear();
            nonAssignedConnections.Clear();
            allConnections.Clear();
            isActive = false;
            UnsubscribeEvents();
        }
    }

    public void WelcomePlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            WelcomePlayer(players[i]);
        }
    }

    public void WelcomePlayer(NetworkConnection cnn)
    {
        if(cnn.IsCreated)
        {
            SendToClient(new NetWelcome() { AssignedTeam = GetSideAsInt(cnn), Role = (int)ClientType.player, isAdmin = IsAdmin(cnn) }, cnn);
        }
    }

    private void OnDestroy() // Shutting down server on destroy.
    {
        Shutdown();
    }

    private void CleanUpConnections()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].IsCreated)
            {
                playerInfo.Remove(players[i]);
                players.RemoveAtSwapBack(i);
                --i;
            }
        }

        spectators.RemoveAll(spectator => !spectator.IsCreated);
        nonAssignedConnections.RemoveAll(cnn => !cnn.IsCreated);

        UpdateConnections();
    }

    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection)) // Checks if a client tries to connect who's not the default connection.
        {
            nonAssignedConnections.Add(c);
            UpdateConnections();
            Debug.Log("Server: New client connected");
            Debug.Log(c.ToString());
        }
    }

    private void UpdateConnections()
    {
        allConnections.Clear();

        for (int i = 0; i < players.Length; i++)
        {
            allConnections.Add(players[i]);
        }

        allConnections.AddRange(spectators);
        allConnections.AddRange(nonAssignedConnections);
    }

    private void UpdateMessagePump()
    {
        try
        {
            DataStreamReader stream; // Reads incoming messages.
            for (int i = 0; i < allConnections.Count; i++)
            {
                NetworkEvent.Type cmd;
                // There are 4 types of network events:
                // Empty = nothing was sent.
                // Connect = connection is made.
                // Data = any net-message sent.
                // Disconnect = connection is severed.

                while ((cmd = driver.PopEventForConnection(allConnections[i], out stream)) != NetworkEvent.Type.Empty)
                {
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        NetUtility.OnData(stream, allConnections[i], this);
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        Debug.Log("Server: Client disconnected from server.");
                        DropConnection(allConnections[i]);
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

    private void DropConnection(NetworkConnection cnn)
    {
        allConnections.Remove(cnn);

        if(spectators.Contains(cnn))
        {
            spectators.Remove(cnn);
            return;
        }

        if(nonAssignedConnections.Contains(cnn))
        {
            nonAssignedConnections.Remove(cnn);
            return;
        }

        int j = -1;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == cnn)
            {
                j = i;
                //players[i] = default(NetworkConnection);
                Debug.Log("player disconnected from game");
                break;
            }
        }
        if(j != -1)
        {
            playerInfo.Remove(players[j]);
            players[j].Disconnect(driver);
            players.RemoveAt(j);
        }
        
        BroadcastMetadata();
    }

    #endregion

    #region ServerSpecific

    private void KeepAlive()
    {
        if (Time.time - lastKeepAlive > KeepAliveTickRate)
        {
            lastKeepAlive = Time.time;
            BroadcastMetadata();
        }
    }

    private void BroadcastMetadata()
    {
        Broadcast(new NetMetadata() { playerCount = PlayerCount, spectatorCount = spectators.Count });
    }

    public void SendToClient(NetMessage msg, NetworkConnection connection) // Send specific message to specific client.
    {
        DataStreamWriter writer; // Able to write net messages.
        driver.BeginSend(connection, out writer); // Finds connection and whom to write message to.
        msg.Serialize(ref writer); // Writes message.
        driver.EndSend(writer); // Transmits message.
    }

    public void Broadcast(NetMessage msg) // Send message to every client.
    {
        ArchiveMessage(msg);
        for (int i = 0; i < allConnections.Count; i++)
        {
            if (allConnections[i].IsCreated)
            {
                Debug.Log($"Server: Sending {msg.Code} to : {allConnections[i].InternalId}");
                SendToClient(msg, allConnections[i]);
            }
        }
    }

    private void ArchiveMessage(NetMessage msg)
    {
        if(msg.GetType() == typeof(NetStartGame))
        {
            messageHistory.Clear();
        }

        if(msg.GetType() != typeof(NetMetadata) && msg.GetType() != typeof(NetUpdateTimer))
        {
            messageHistory.Add(msg);
        }
    }

    private void ClearMessageHistory(PlayerType? winner, GameOverCondition endGameCondition)
    {
        messageHistory.Clear();
    }

    public IEnumerator SendGameState(NetworkConnection connection)
    {
        if (messageHistory.Count > 0)
        {
            Debug.Log("Sending history to client: " + messageHistory.Count);
            float delay = 0.02f;

            SendToClient(new NetChangeLoadGameStatus(), connection);
            yield return new WaitForSeconds(delay);

            foreach (NetMessage msg in messageHistory)
            {
                SendToClient(msg, connection);
                yield return new WaitForSeconds(delay);
            }

            SendToClient(new NetChangeLoadGameStatus(), connection);
        }
    }

    #endregion

    private void SubscribeEvents()
    {
        GameplayEvents.OnGameOver += ClearMessageHistory;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameOver -= ClearMessageHistory;
    }
}