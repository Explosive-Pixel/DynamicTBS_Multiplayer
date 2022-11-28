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

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public NetworkDriver driver;
    private NativeList<NetworkConnection> connections;

    private bool isActive = false;
    private const float KeepAliveTickRate = 20f; // Constant tick rate, so connection won't time out.
    private float lastKeepAlive = 0f; // Timestamp for last connection.

    public Action connectionDropped;

    public int playerCount = 0;

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

        connections = new NativeList<NetworkConnection>(2, Allocator.Persistent); // Allows up to two connections at a time.
        isActive = true;
    }

    public void Shutdown() // For shutting down the server.
    {
        if (isActive)
        {
            driver.Dispose();
            connections.Dispose();
            isActive = false;
        }
    }

    private void OnDestroy() // Shutting down server on destroy.
    {
        Shutdown();
    }

    private void CleanUpConnections()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    private void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection)) // Checks if a client tries to connect who's not the default connection.
        {
            connections.Add(c);
            Debug.Log("Server: New client connected");
            Debug.Log(c.ToString());
        }
    }

    private void UpdateMessagePump()
    {
        try
        {
            DataStreamReader stream; // Reads incoming messages.
            for (int i = 0; i < connections.Length; i++)
            {
                NetworkEvent.Type cmd;
                // There are 4 types of network events:
                // Empty = nothing was sent.
                // Connect = connection is made.
                // Data = any net-message sent.
                // Disconnect = connection is severed.

                while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
                {
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        NetUtility.OnData(stream, connections[i], this);
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        Debug.Log("Server: Client disconnected from server.");
                        connections[i] = default(NetworkConnection);
                        connectionDropped?.Invoke();
                        Shutdown(); // Does not happen usually, only because this is a 2 person game.
                    }
                }
            }
        }
        catch (ObjectDisposedException)
        {
            return;
        }
    }

    #endregion

    #region ServerSpecific

    private void KeepAlive()
    {
        if (Time.time - lastKeepAlive > KeepAliveTickRate)
        {
            lastKeepAlive = Time.time;
            Broadcast(new NetKeepAlive());
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
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].IsCreated)
            {
                Debug.Log($"Server: Sending {msg.Code} to : {connections[i].InternalId}");
                SendToClient(msg, connections[i]);
            }
        }
    }

    #endregion
}