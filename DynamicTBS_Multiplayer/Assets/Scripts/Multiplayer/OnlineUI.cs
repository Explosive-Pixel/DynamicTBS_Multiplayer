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
        BackToMainMenuButton();
    }

    public void OnlineHostButton()
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8007, ClientType.player);
        AddMessageHandlers();
        onlineHostCanvas.SetActive(true);
        onlineMenuCanvas.SetActive(false);
    }

    public void OnlineConnectAsPlayerButton()
    {
        ConnectAsClient(ClientType.player);
    }

    public void OnlineConnectAsSpectatorButton()
    {
        ConnectAsClient(ClientType.spectator);
    }

    public void BackToMainMenuButton()
    {
        server.Shutdown();
        client.Shutdown();
        RemoveMessageHandlers();
    }

    private void ConnectAsClient(ClientType clientType)
    {
        client.Init(addressInput.text, 8007, clientType);
        AddMessageHandlers();
        onlineClientCanvas.SetActive(true);
        onlineMenuCanvas.SetActive(false);
    }

    private void AddMessageHandlers()
    {
        onlineGameManager.AddComponent<ClientMessageHandler>();
        onlineGameManager.AddComponent<ServerMessageHandler>();
    }

    private void RemoveMessageHandlers()
    {
        if(onlineGameManager.TryGetComponent(out ClientMessageHandler cmh))
        {
            Destroy(cmh);
        }

        if (onlineGameManager.TryGetComponent(out ServerMessageHandler smh))
        {
            Destroy(smh);
        }
    }
}