using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Networking.Transport;

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

    public int PlayerCount { get { return isActive ? players.Length : 0; } }
    public int HostSide { get { return hostSide; } set { hostSide = value; } }

    private NetworkDriver driver;
    private NativeList<NetworkConnection> players;
    private readonly List<NetworkConnection> spectators = new List<NetworkConnection>();
    private readonly List<NetworkConnection> allConnections = new List<NetworkConnection>();
    private readonly List<NetworkConnection> nonAssignedConnections = new List<NetworkConnection>();

    private bool isActive = false;
    private const float KeepAliveTickRate = 20f; // Constant tick rate, so connection won't time out.
    private float lastKeepAlive = 0f; // Timestamp for last connection.

    private Action connectionDropped;
    private int hostSide = 0;

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
        hostSide = 0;
    }

    public void RegisterAs(NetworkConnection c, ClientType role)
    {
        if(nonAssignedConnections.Contains(c))
        {
            if(role == ClientType.player)
            {
                players.Add(c);
            } else
            {
                spectators.Add(c);
                Broadcast(new NetMetadata() { spectatorCount = spectators.Count });

            }
            nonAssignedConnections.Remove(c);
            UpdateConnections();
        }
    }

    public NetworkConnection? FindOtherConnection(NetworkConnection host)
    {
        Debug.Log("Player connections: " + players.Length);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != host)
            {
                return players[i];
            }
        }
        return null;
    }

    public int GetNonHostSide()
    {
        if (hostSide == 1)
            return 2;
        if (hostSide == 2)
            return 1;
        return 0;
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
                        Shutdown(); // Does not happen usually, only because this is a 2 person game.
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

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == cnn)
            {
                players[i] = default(NetworkConnection);
                return;
            }
        }
    }

    #endregion

    #region ServerSpecific

    private void KeepAlive()
    {
        if (Time.time - lastKeepAlive > KeepAliveTickRate)
        {
            lastKeepAlive = Time.time;
            //Broadcast(new NetKeepAlive());
            Broadcast(new NetMetadata() { spectatorCount = spectators.Count });
        }
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
        for (int i = 0; i < allConnections.Count; i++)
        {
            if (allConnections[i].IsCreated)
            {
                Debug.Log($"Server: Sending {msg.Code} to : {allConnections[i].InternalId}");
                SendToClient(msg, allConnections[i]);
            }
        }
    }

    #endregion
}