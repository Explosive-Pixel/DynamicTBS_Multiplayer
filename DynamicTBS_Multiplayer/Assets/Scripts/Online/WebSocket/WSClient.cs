using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class WSClient : MonoBehaviour
{
    WebSocket websocket;

    private string hostname;

    private bool destroyed = false;
    private readonly Queue<WSMessage> unsendMsgs = new();

    public bool Active { get; private set; } = false;

    private float timer = 0f;
    private const float keepAliveInterval = 25f; // in Seconds

    #region SingletonImplementation

    public static WSClient Instance { set; get; }

    private void Awake()
    {
        Instance = this;
        destroyed = false;
    }

    #endregion

    public void Init(string hostname, ClientType role, string userName, LobbyId lobby, bool createLobby)
    {
        this.hostname = hostname;

        Client.Init(role, userName, lobby);

        CreateWebsocketConnection(createLobby, false);
    }

    private void CreateWebsocketConnection(bool createLobby, bool isReconnect)
    {
        Client.ConnectionStatus = ConnectionStatus.ATTEMPT_CONNECTION;

        CreateWebsocketConnectionAsync(createLobby, isReconnect);
    }

    private async void CreateWebsocketConnectionAsync(bool createLobby, bool isReconnect)
    {
        Debug.Log("Trying to connect to server with hostname " + hostname);

        websocket = new WebSocket(hostname);

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
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            // Debug.Log("Received msg:\n" + message);
            MessageReceiver.ReceiveMessage(message);
        };

        await websocket.Connect();
    }

    private void TryReconnect()
    {
        if (Client.Active && !Active && !destroyed)
        {
            CreateWebsocketConnection(false, true);
        }
    }

    void Update()
    {
        if (!Active)
            return;

        timer += Time.deltaTime;
        if (timer >= keepAliveInterval)
        {
            SendMessage(new WSMsgKeepAlive());
            timer = 0f;
        }

#if !UNITY_WEBGL || UNITY_EDITOR
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
        destroyed = true;

        Client.Reset();

        if (websocket != null)
            await websocket.Close();
    }

}
