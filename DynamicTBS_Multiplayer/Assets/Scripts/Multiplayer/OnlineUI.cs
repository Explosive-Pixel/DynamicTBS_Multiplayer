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

    private GameObject gameplayCanvas;

    private void Awake()
    {
        gameplayCanvas = GameObject.Find("GameManager").GetComponent<GameManager>().GetGameplayCanvas();
    }

    public void OnlineHostButton()
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8007);
        AddMessageHandlers();
    }

    public void OnlineConnectButton()
    {
        client.Init(addressInput.text, 8007);
        AddMessageHandlers();
    }

    public void BackToMainMenuButton()
    {
        server.Shutdown();
        client.Shutdown();
        RemoveMessageHandlers();
    }

    private void AddMessageHandlers()
    {
        gameplayCanvas.AddComponent<ClientMessageHandler>();
        gameplayCanvas.AddComponent<ServerMessageHandler>();
    }

    private void RemoveMessageHandlers()
    {
        Destroy(gameplayCanvas.GetComponent<ClientMessageHandler>());
        Destroy(gameplayCanvas.GetComponent<ServerMessageHandler>());
    }
}