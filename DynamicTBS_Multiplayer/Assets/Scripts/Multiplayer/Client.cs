using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour
{
    #region SingletonImplementation

    public static Client Instance { set; get; }
    private GameObject onlineGameManagerObject;

    private void Awake()
    {
        Instance = this;
        Shutdown();
        onlineGameManagerObject = this.gameObject;
        DontDestroyOnLoad(onlineGameManagerObject);
    }

    #endregion

    public NetworkDriver driver;
    private NetworkConnection connection;

    private string ip;
    private ushort port;
    private ClientType clientType;

    private bool isLoadingGame = false;
    public bool IsLoadingGame { get { return isLoadingGame; } }

    private bool isInitialized = false;
    private bool isActive = false;
    private bool isConnected = false;
    public bool IsInitialized { get { return isInitialized; } }
    public bool IsActive { get { return isActive;  } }
    public bool IsConnected { get { return isConnected; } }

    private bool connectionAccepted = true;
    public bool ConnectionAccepted { get { return connectionAccepted;  } }

    public Action connectionDropped;

    public ClientType role;
    public PlayerType side;

    public bool isAdmin;

    #region Init & Destroy

    public void Init(string ip, ushort port, ClientType clientType) // Initiation method.
    {
        this.ip = ip;
        this.port = port;
        this.clientType = clientType;

        driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.Parse(ip, port); // Specific endpoint for connection.

        connection = driver.Connect(endPoint); // Connecting based on the endpoint that was just created.
        Debug.Log("Client: Attempting to connect to server on " + endPoint.Address);

        isActive = true;
        role = clientType;
        isAdmin = false;
        isInitialized = true;

        RegisterToEvent();
    }

    public void Shutdown() // For shutting down the server.
    {
        if (isActive)
        {
            Debug.Log("Shutting down Client.");
            UnregisterToEvent();
            connection.Disconnect(driver);
            driver.Dispose();
            isActive = false;
            isConnected = false;
            //connection = default(NetworkConnection);
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
            isConnected = false;
            // Shutdown();
            Reconnect();
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
                if (cmd == NetworkEvent.Type.Connect)
                {
                    Debug.Log("Client: We're connected! Role: " + role);
                    isConnected = true;
                    SendToServer(new NetWelcome() { Role = (int)role });
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    NetUtility.OnData(stream, default(NetworkConnection));
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client: Client disconnected from server.");
                    // TODO: Show disconnect message on UI
                    isConnected = false;
                    connection = default(NetworkConnection);
                    //connection.Disconnect(driver);
                    connectionDropped?.Invoke();
                    Shutdown();
                }
            }
        }
        catch (Exception)
        {
            return;
        }
    }

    public bool ShouldReadMessage(PlayerType playerType)
    {
        return side != playerType || IsLoadingGame;
    }

    public bool ShouldSendMessage(PlayerType playerType)
    {
        return side == playerType && !IsLoadingGame;
    }

    public void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    private void Reconnect()
    {
        Init(ip, port, clientType);
    }

    private void OnKeepAlive(NetMessage nm)
    {
        //isConnected = true;
        SendToServer(nm); // Sends message back to keep both sides alive.
    }

    private void ToggleIsLoadingGame(NetMessage nm)
    {
        isLoadingGame = !isLoadingGame;
        GameEvents.IsGameLoading(isLoadingGame);
    }

    private void SetConnectionForbidden(NetMessage msg)
    {
        connectionAccepted = false;
        Shutdown();
    }

    #region Events

    private void RegisterToEvent()
    {
        NetUtility.C_CONNECTION_FORBIDDEN += SetConnectionForbidden;
        NetUtility.C_CHANGE_LOAD_GAME_STATUS += ToggleIsLoadingGame;
        NetUtility.C_METADATA += OnKeepAlive;
    }

    private void UnregisterToEvent()
    {
        NetUtility.C_CONNECTION_FORBIDDEN -= SetConnectionForbidden;
        NetUtility.C_CHANGE_LOAD_GAME_STATUS -= ToggleIsLoadingGame;
        NetUtility.C_METADATA -= OnKeepAlive;
    }

    #endregion
}