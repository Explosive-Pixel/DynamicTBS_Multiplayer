using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class WSClient : MonoBehaviour
{
    WebSocket websocket;

    private string ipAdress;
    private ushort port;
    private bool destroyed = false;
    private readonly Queue<WSMessage> unsendMsgs = new();

    public bool Active { get; private set; } = false;

    #region SingletonImplementation

    public static WSClient Instance { set; get; }

    private void Awake()
    {
        Instance = this;
        destroyed = false;
    }

    #endregion

    public void Init(string ipAdress, ushort port, ClientType role, string userName, LobbyId lobby, bool createLobby)
    {
        this.ipAdress = ipAdress;
        this.port = port;

        Client.Init(role, userName, lobby);

        CreateWebsocketConnectionAsync(createLobby, false);
    }

    private async void CreateWebsocketConnectionAsync(bool createLobby, bool isReconnect)
    {
        Client.ConnectionStatus = ConnectionStatus.ATTEMPT_CONNECTION;
        var resolvedIp = await IPResolver.GetPublicIpV6Address(ipAdress);

        if (resolvedIp == null || resolvedIp.Length == 0)
        {
            Debug.Log("Error: Failed to resolve ip for adress " + ipAdress);
            return;
        }

        Debug.Log("Trying to connect to server with ip " + resolvedIp);
        websocket = new WebSocket("ws://[" + resolvedIp + "]:" + port);

        websocket.OnOpen += () =>
        {
            Debug.Log("Successfully connected to server!");
            Active = true;
            Client.TryJoinLobby(createLobby, isReconnect);

            if (isReconnect)
            {
                while (unsendMsgs.Count > 0)
                {
                    SendMessage(unsendMsgs.Dequeue());
                }
            }
        };

        websocket.OnError += (e) =>
        {
            Client.ConnectionStatus = ConnectionStatus.UNCONNECTED;
            Debug.Log("Error! Cannot connect to server: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Active = false;
            Client.ConnectionStatus = ConnectionStatus.UNCONNECTED;
            Debug.Log("Connection to server closed!");


            TryReconnect();
        };

        websocket.OnMessage += (bytes) =>
        {
            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received msg:\n" + message);
            MessageReceiver.ReceiveMessage(message);
        };

        await websocket.Connect();
    }

    private void TryReconnect()
    {
        if (Client.Active && !Active && !destroyed)
        {
            CreateWebsocketConnectionAsync(false, true);
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (Active)
            websocket.DispatchMessageQueue();
#endif
    }

    public async void SendMessage(WSMessage msg)
    {
        if (!Active && websocket.State != WebSocketState.Open)
        {
            unsendMsgs.Enqueue(msg);
            return;
        }

        // Sending plain text
        await websocket.SendText(msg.Serialize());
    }

    public void LoadGame(List<WSMessage> msgHistory)
    {
        StartCoroutine(ProcessLoadGame(msgHistory));
    }

    private IEnumerator ProcessLoadGame(List<WSMessage> msgHistory)
    {
        foreach (var msg in msgHistory)
        {
            yield return new WaitForEndOfFrame();
            msg.HandleMessage();
        }

        Client.ToggleIsLoadingGame();
    }

    private async void OnDestroy()
    {
        Debug.Log("Destroy WS Object");
        destroyed = true;

        Client.Reset();

        if (websocket != null)
            await websocket.Close();
    }

}
