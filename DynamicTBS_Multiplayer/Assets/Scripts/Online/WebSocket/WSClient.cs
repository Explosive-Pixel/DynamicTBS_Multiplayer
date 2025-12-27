using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;
using System.Collections;

public enum ConnectionState
{
    DICONNECTED,
    CONNECTING,
    CONNECTED,
    RECONNECTING,
    DEAD
}

public class WSClient : MonoBehaviour
{
    public ConnectionState ConnectionState { get { return state; } }

    #region Private attributes

    private WebSocket websocket;
    private string hostname;

    private const int retryIntervalMs = 3000;
    private const int maxRetries = 5;

    private const int reconnectBaseDelayMs = 3000;
    private const int reconnectMaxDelayMs = 30000;
    private const int reconnectMaxRetries = 20;

    private bool destroyed;
    private int reconnectAttempts = 0;
    private int delay = reconnectBaseDelayMs;

    private bool gameIsUpToDate = true;

    private ConnectionState state = ConnectionState.DICONNECTED;

    private readonly Queue<WSMessage> outgoingQueue = new();
    private PendingMessage currentMessage;

    private class PendingMessage
    {
        public WSMessage msg;
        public int retries;
        public float lastSentTime;
    }

    private readonly Dictionary<string, WSMessage> processedMessages = new();

    private float keepAliveTimer;
    private const float keepAliveInterval = 20f;

    private bool IsConnected =>
        websocket != null && websocket.State == WebSocketState.Open;

    #endregion

    #region Singleton

    public static WSClient Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        hostname = ConfigManager.Instance.Hostname;

        GameplayEvents.OnGameOver += ResetMsgHistory;

        _ = ConnectAsync(false);
    }

    #endregion

    // ===================== CONNECTION =====================

    private async Task ConnectAsync(bool isReconnect)
    {
        if (destroyed || state == ConnectionState.CONNECTING)
            return;

        state = isReconnect ? ConnectionState.RECONNECTING : ConnectionState.CONNECTING;

        CleanupWebSocket();

        websocket = new WebSocket(hostname);

        websocket.OnOpen += OnWebSocketOpen;
        websocket.OnClose += OnWebSocketClosed;
        websocket.OnError += OnWebSocketError;
        websocket.OnMessage += HandleMessage;

        await websocket.Connect();
    }

    private void CleanupWebSocket()
    {
        if (websocket == null) return;

        websocket.OnOpen -= OnWebSocketOpen;
        websocket.OnClose -= OnWebSocketClosed;
        websocket.OnError -= OnWebSocketError;
        websocket.OnMessage -= HandleMessage;

        //await websocket.Close();
        websocket = null;
    }

    private void OnWebSocketOpen()
    {
        if (state == ConnectionState.RECONNECTING)
        {
            Debug.Log("Reconnected to websocket!");
            reconnectAttempts = 0;
            delay = reconnectBaseDelayMs;
            Client.Reconnect();
        }
        state = ConnectionState.CONNECTED;

        Debug.Log("WebSocket connected");
    }

    private void OnWebSocketClosed(WebSocketCloseCode closeCode)
    {
        Debug.Log($"WebSocket closed: {closeCode}");
        state = ConnectionState.DICONNECTED;

        gameIsUpToDate = false;
        _ = TryReconnect();
    }

    private void OnWebSocketError(string error)
    {
        Debug.LogError($"WebSocket error: {error}");
    }


    // ===================== RECONNECT =====================

    private async Task TryReconnect()
    {
        if (state == ConnectionState.RECONNECTING || destroyed)
            return;

        state = ConnectionState.RECONNECTING;

        if (!destroyed && reconnectAttempts < reconnectMaxRetries)
        {
            reconnectAttempts++;
            Debug.Log($"Reconnect attempt {reconnectAttempts}");

            await Task.Delay(delay);
            delay = Mathf.Min(delay * 2, reconnectMaxDelayMs);

            await ConnectAsync(true);
        }
        else if (reconnectAttempts == reconnectMaxRetries)
        {
            Debug.LogError("Reconnect permanently failed");
            state = ConnectionState.DEAD;
        }
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
            if (!processedMessages.ContainsKey(msg.uuid))
            {
                UpdateProcessedMessages(msg);
                Debug.Log("Reloading message " + msg + " with uuid " + msg.uuid);
                msg.HandleMessage();
            }
        }

        Client.ToggleIsLoadingGame();
        gameIsUpToDate = true;
    }

    // ===================== UPDATE =====================

    private void Update()
    {
        if (!IsConnected || destroyed)
            return;

        ProcessQueue();
        KeepAlive();

#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    private void KeepAlive()
    {
        keepAliveTimer += Time.deltaTime;
        if (keepAliveTimer >= keepAliveInterval)
        {
            SendMessage(new WSMsgKeepAlive());
            keepAliveTimer = 0f;
        }
    }

    // ===================== MESSAGING =====================

    public void SendMessage(WSMessage msg)
    {
        outgoingQueue.Enqueue(msg);
        TrySendNext();
    }

    public async Task SendDirectly(WSMessage msg)
    {
        await websocket.SendText(msg.Serialize());
    }

    private void HandleMessage(byte[] bytes)
    {
        var json = System.Text.Encoding.UTF8.GetString(bytes);
        var msg = WSMessage.Deserialize(json);

        if (msg == null)
        {
            Debug.Log("Message '" + json + "' could not be serialized.");
            return;
        }

        if (msg.code == WSMessageCode.WSMsgAcknowledgeCode)
        {
            Debug.Log("Received Acknowledgement for message with uuid: " + ((WSMsgAcknowledge)msg).messageUuid);
            HandleAck(((WSMsgAcknowledge)msg).messageUuid);
            return;
        }

        Debug.Log("Received message: " + msg + " with uuid " + msg.uuid);
        SendAck(msg.uuid);
        UpdateProcessedMessages(msg);
        MessageReceiver.ReceiveMessage(msg);
        msg.HandleMessage();
    }

    private void HandleAck(string uuid)
    {
        if (currentMessage == null || currentMessage.msg.uuid != uuid)
            return;

        currentMessage = null;
        TrySendNext();
    }

    private void ProcessQueue()
    {
        if (currentMessage == null || !gameIsUpToDate)
            return;

        float now = Time.time * 1000f;
        if (now - currentMessage.lastSentTime < retryIntervalMs)
            return;

        if (currentMessage.retries >= maxRetries)
        {
            Debug.LogWarning("ACK timeout → forcing reconnect");
            _ = websocket.Close();
            return;
        }

        SendCurrent();
    }

    private void TrySendNext()
    {
        if (currentMessage != null || outgoingQueue.Count == 0)
            return;

        currentMessage = new PendingMessage
        {
            msg = outgoingQueue.Dequeue(),
            retries = 0,
            lastSentTime = 0
        };

        SendCurrent();
    }

    private async void SendCurrent()
    {
        if (!IsConnected || currentMessage == null)
            return;

        currentMessage.lastSentTime = Time.time * 1000f;
        currentMessage.retries++;

        await SendDirectly(currentMessage.msg);
        Debug.Log($"Sent message ({currentMessage.retries}/{maxRetries}): {currentMessage.msg.Serialize()}");
    }

    private async void SendAck(string uuid)
    {
        await SendDirectly(
            new WSMsgAcknowledge { messageUuid = uuid }
        );
        Debug.Log($"Sent acknowlesge message for uuid: {uuid}");
    }

    private void UpdateProcessedMessages(WSMessage msg)
    {
        if (WSMessage.Record(msg))
        {
            processedMessages.Add(msg.uuid, msg);
        }
    }

    // ===================== CLEANUP =====================

    private void ResetMsgHistory(PlayerType? _, GameOverCondition __)
    {
        processedMessages.Clear();
    }

    private async void OnDestroy()
    {
        if (Instance != this)
            return;

        destroyed = true;

        Client.Reset();
        GameplayEvents.OnGameOver -= ResetMsgHistory;
        await CleanupWebSocket();
    }
}
