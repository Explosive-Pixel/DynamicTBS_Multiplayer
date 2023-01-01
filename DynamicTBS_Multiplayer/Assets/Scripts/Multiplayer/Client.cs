using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour
{
    #region SingletonImplementation

    public static Client Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public NetworkDriver driver;
    private NetworkConnection connection;

    private bool isActive = false;
    private bool isConnected = false;
    public bool IsActive { get { return isActive;  } }
    public bool IsConnected { get { return isConnected; } }

    public Action connectionDropped;

    public PlayerType side;

    #region Init & Destroy

    public void Init(string ip, ushort port) // Initiation method.
    {
        driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.Parse(ip, port); // Specific endpoint for connection.

        connection = driver.Connect(endPoint); // Connecting based on the endpoint that was just created.
        Debug.Log("Client: Attempting to connect to server on " + endPoint.Address);

        isActive = true;

        RegisterToEvent();
    }

    public void Shutdown() // For shutting down the server.
    {
        if (isActive)
        {
            UnregisterToEvent();
            driver.Dispose();
            isActive = false;
            isConnected = false;
            connection = default(NetworkConnection);
        }
    }

    private void OnDestroy() // Shutting down server on destroy.
    {
        Shutdown();
    }

    #endregion

    private void Update()
    {
        if (!isActive) return;

        driver.ScheduleUpdate().Complete(); // Makes sure driver processed all incoming messages.
        CheckAlive(); // Checks if connection to server is alive.
        UpdateMessagePump(); // Check for messages and if server has to reply.
    }

    private void CheckAlive()
    {
        if (!connection.IsCreated && isActive) // If no connections is created, but client is active, something went wrong.
        {
            Debug.Log("Client: Something went wrong. Lost connection to server.");
            connectionDropped?.Invoke();
            Shutdown();
        }
    }

    private void UpdateMessagePump()
    {
        try
        {
            DataStreamReader stream; // Reads incoming messages.
            NetworkEvent.Type cmd;

            while (connection != null && (cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
            {
                Debug.Log("Client: Reading message " + cmd);
                if (cmd == NetworkEvent.Type.Connect)
                {
                    Debug.Log("Client: We're connected!");
                    SendToServer(new NetWelcome());
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    NetUtility.OnData(stream, default(NetworkConnection));
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client: Client disconnected from server.");
                    connection = default(NetworkConnection);
                    connectionDropped?.Invoke();
                    Shutdown();
                }
            }
        }
        catch (ObjectDisposedException)
        {
            Debug.Log("Client: Error");
            return;
        }
    }

    public void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    #region Events

    private void RegisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE += OnKeepAlive;
    }

    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
    }

    private void OnKeepAlive(NetMessage nm)
    {
        isConnected = true;
        SendToServer(nm); // Sends message back to keep both sides alive.
    }

    #endregion
}