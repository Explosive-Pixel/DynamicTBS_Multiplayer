using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;

public class ServerCanvasHandler : MonoBehaviour
{

    [SerializeField] private Text connectionCount;
    [SerializeField] private Text lobbyCount;
    [SerializeField] private Text status;
    [SerializeField] private Text ip;

    // Update is called once per frame
    void Update()
    {
        if (!OnlineServer.Instance)
            return;

        status.text = "Server is active: " + OnlineServer.Instance.IsActive;
        ip.text = "Server IP: " + OnlineServer.Instance.IP;
        connectionCount.text = "Connected Clients: " + OnlineServer.Instance.ConnectionCount;
        lobbyCount.text = "Active Lobbies: " + OnlineServer.Instance.LobbyCount;
    }
}
