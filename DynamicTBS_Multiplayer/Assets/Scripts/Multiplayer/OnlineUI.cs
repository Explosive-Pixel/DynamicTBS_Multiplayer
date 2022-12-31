using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OnlineUI : MonoBehaviour
{
    public Server server;
    public Client client;

    [SerializeField] InputField addressInput;
    [SerializeField] GameObject onlineMenuCanvas;
    [SerializeField] GameObject onlineHostCanvas;
    [SerializeField] GameObject onlineClientCanvas;

    private GameObject onlineGameManager;

    private void Awake()
    {
        onlineGameManager = GameObject.Find("OnlineGameManager");
    }

    public void OnlineHostButton()
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8007);
        AddMessageHandlers();
        GameManager.StartRecording();
        onlineHostCanvas.SetActive(true);
        onlineMenuCanvas.SetActive(false);
    }

    public void OnlineConnectButton()
    {
        client.Init(addressInput.text, 8007);
        AddMessageHandlers();
        GameManager.StartRecording();
        onlineClientCanvas.SetActive(true);
        onlineMenuCanvas.SetActive(false);
    }

    public void BackToMainMenuButton()
    {
        server.Shutdown();
        client.Shutdown();
        RemoveMessageHandlers();
    }

    private void AddMessageHandlers()
    {
        onlineGameManager.AddComponent<ClientMessageHandler>();
        onlineGameManager.AddComponent<ServerMessageHandler>();
    }

    private void RemoveMessageHandlers()
    {
        Destroy(onlineGameManager.GetComponent<ClientMessageHandler>());
        Destroy(onlineGameManager.GetComponent<ServerMessageHandler>());
    }
}