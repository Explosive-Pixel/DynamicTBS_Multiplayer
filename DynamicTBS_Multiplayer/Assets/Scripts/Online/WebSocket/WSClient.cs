using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class WSClient : MonoBehaviour
{
    WebSocket websocket;

    bool active = false;

    // Start is called before the first frame update
    public async void Init(ClientType role, string userName, LobbyId lobby, bool createLobby)
    {
        websocket = new WebSocket("ws://localhost:8007");

        websocket.OnOpen += () =>
        {
            Debug.Log("Successfully connected to server!");
            active = true;
            Client.Init(this, role, userName, lobby, createLobby);
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            active = false;
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            // Debug.Log(bytes);

            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received msg:\n" + message);
            MessageReceiver.ReceiveMessage(message);
            // Debug.Log("OnMessage! " + message);
        };

        // Keep sending messages at every 0.3s
        //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        // waiting for messages
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (active)
            websocket.DispatchMessageQueue();
#endif
    }

    public async void SendMessage(WSMessage msg)
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            //await websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            await websocket.SendText(msg.Serialize());
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
            msg.HandleMessage();
        }

        Client.ToggleIsLoadingGame();
    }

    private async void OnDestroy()
    {
        Client.Reset();

        if (websocket != null)
            await websocket.Close();
    }

}
