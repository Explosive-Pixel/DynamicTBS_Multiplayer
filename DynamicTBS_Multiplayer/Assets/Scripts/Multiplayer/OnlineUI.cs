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
    [SerializeField] GameObject onlineClientCanvas;

    private GameObject onlineGameManager;

    private void Awake()
    {
        onlineGameManager = GameObject.Find("OnlineGameManager");
        BackToMainMenuButton();
    }

    public void OnlineHostAsPlayerButton()
    {
        OnlineHostButton(ClientType.player);
    }

    public void OnlineHostAsSpectatorButton()
    {
        OnlineHostButton(ClientType.spectator);
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

    private void OnlineHostButton(ClientType clientType)
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8007, clientType);
        ReworkConnection();
    }

    private void ConnectAsClient(ClientType clientType)
    {
        client.Init(addressInput.text, 8007, clientType);
        ReworkConnection();
    }

    private void ReworkConnection()
    {
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