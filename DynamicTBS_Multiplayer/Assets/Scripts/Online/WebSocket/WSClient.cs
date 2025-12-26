using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System;

public class WSClient : MonoBehaviour
{
    WebSocket websocket;

    private string hostname;
    public int retryIntervalMs = 3000;
    public int maxRetries = 5;

    private bool destroyed = false;

    private readonly Queue<WSMessage> outgoingQueue = new();
    private PendingMessage currentMessage;

    [Serializable]
    private class PendingMessage
    {
        public WSMessage msg;
        public int retries;
        public float lastSentTime;
    }

    private readonly List<WSMessage> msgHistory = new();

    private float timer = 0f;
    private const float keepAliveInterval = 20f; // in Seconds

    private bool IsConnectedToServer { get { return websocket.State == WebSocketState.Open; } }

    #region SingletonImplementation

    public static WSClient Instance { set; get; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            destroyed = false;

            hostname = ConfigManager.Instance.Hostname;
            CreateWebsocketConnection(false);

            GameplayEvents.OnGameOver += ResetMsgHistory;
        }
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

            //TryReconnect(); // TODO: Timer einbauen
        };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            WSMessage msg = WSMessage.Deserialize(message);

            if (msg != null)
            {

                if (msg.code == WSMessageCode.WSMsgAcknowledgeCode)
                {
                    Debug.Log("Received Acknowledgement for message with uuid: " + ((WSMsgAcknowledge)msg).messageUuid);
                    HandleAck(((WSMsgAcknowledge)msg).messageUuid);
                    return;
                }

                Debug.Log("Received message: " + msg + " with uuid " + msg.uuid);
                SendAck(msg.uuid);
                UpdateMsgHistory(msg);
                MessageReceiver.ReceiveMessage(msg);
                msg.HandleMessage();
            }
            else
            {
                Debug.Log("Message '" + message + "' could not be serialized.");
            }
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

        ProcessQueue();

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

    public void SendMessage(WSMessage msg)
    {
        /*if (!IsConnectedToServer)
        {
            unsendMsgs.Enqueue(msg);
            return;
        }

        Debug.Log("Sending message: " + msg.Serialize());
        await websocket.SendText(msg.Serialize());*/
        outgoingQueue.Enqueue(msg);
        TrySendNext();
    }

    private void HandleAck(string messageUuid)
    {
        if (currentMessage == null) return;
        if (currentMessage.msg.uuid != messageUuid) return;

        currentMessage = null;
        TrySendNext();
    }

    private void ProcessQueue()
    {
        if (currentMessage == null) return;

        float now = Time.time * 1000f;
        if (now - currentMessage.lastSentTime < retryIntervalMs) return;

        if (currentMessage.retries >= maxRetries)
        {
            Debug.LogWarning("Message not acknowledged, disconnecting from server.");
            websocket.Close();
            return;
        }

        ResendCurrent();
    }

    private void TrySendNext()
    {
        if (currentMessage != null) return;
        if (outgoingQueue.Count == 0) return;

        var next = outgoingQueue.Dequeue();
        currentMessage = new PendingMessage
        {
            msg = next,
            retries = 0,
            lastSentTime = 0
        };

        SendCurrent();
    }

    private async void SendAck(string messageUuid)
    {
        string ackMsg = new WSMsgAcknowledge { messageUuid = messageUuid }.Serialize();
        await websocket.SendText(ackMsg);
        Debug.Log($"Sent acknowlesge message for uuid: {messageUuid}");
    }

    private async void SendCurrent()
    {
        if (currentMessage == null) return;

        currentMessage.lastSentTime = Time.time * 1000f;
        currentMessage.retries++;

        await websocket.SendText(currentMessage.msg.Serialize());
        Debug.Log($"Sent message ({currentMessage.retries}/{maxRetries}): {currentMessage.msg.Serialize()}");
    }

    private void ResendCurrent()
    {
        Debug.Log($"Retrying ({currentMessage.retries + 1}/{maxRetries}) for message {currentMessage.msg.uuid}");
        SendCurrent();
    }

    private void UpdateMsgHistory(WSMessage msg)
    {
        if (WSMessage.Record(msg))
        {
            msgHistory.Add(msg);
        }
    }

    private bool IsNewMessage(WSMessage msg)
    {
        return msgHistory.Find(hm => hm.uuid == msg.uuid) == null;
    }

    private void ResetMsgHistory(PlayerType? winner, GameOverCondition endGameCondition)
    {
        msgHistory.Clear();
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
            if (IsNewMessage(msg))
            {
                UpdateMsgHistory(msg);
                msg.HandleMessage();
            }
        }

        Client.ToggleIsLoadingGame();
    }

    private async void OnDestroy()
    {
        if (Instance != this)
            return;

        destroyed = true;

        Client.Reset();

        GameplayEvents.OnGameOver -= ResetMsgHistory;

        if (websocket != null)
            await websocket.Close();
    }
}
