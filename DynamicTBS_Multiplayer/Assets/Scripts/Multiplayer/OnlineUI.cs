using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineUI : MonoBehaviour
{
    public Server server;
    public Client client;

    [SerializeField] InputField addressInput;

    public void OnlineHostButton()
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8007);
    }

    public void OnlineConnectButton()
    {
        client.Init(addressInput.text, 8007);
    }

    public void BackToMainMenuButton()
    {
        server.Shutdown();
        client.Shutdown();
    }
}