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

    private float timer = 0f;
    private const float keepAliveInterval = 25f; // in Seconds

    private bool IsConnectedToServer { get { return websocket.State == WebSocketState.Open; } }

    #region SingletonImplementation

    public static WSClient Instance { set; get; }

    private void Awake()
    {
        Instance = this;
        destroyed = false;

        hostname = ConfigManager.Instance.Hostname;
        CreateWebsocketConnection(false);
    }

    #endregion

    private void CreateWebsocketConnection(bool isReconnect)
    {
        Client.ConnectionStatus = ConnectionStatus.ATTEMPT_CONNECTION;

        CreateWebsocketConnectionAsync(isReconnect);
    }

    private async void CreateWebsocketConnectionAsync(bool isReconnect)
    {
        Debug.Log("Trying to connect to server with hostname " + hostname);

        websocket = new WebSocket(hostname);

        websocket.OnOpen += () =>
        {
            Debug.Log("Successfully connected to server!");
            Client.Active = true;
            Client.ConnectionStatus = ConnectionStatus.CONNECTED;

            if (isReconnect)
            {
                Client.Reconnect();

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
        if (Client.Active && !IsConnectedToServer && !destroyed)
        {
            CreateWebsocketConnection(true);
        }
    }

    void Update()
    {
        if (!IsConnectedToServer)
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
        if (!IsConnectedToServer)
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
