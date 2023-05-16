using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineMetadata : MonoBehaviour
{
    [SerializeField] private GameObject multiplayerMetadata;

    [SerializeField] private Text infoText;

    private void Awake()
    {
        infoText.text = "";
    }

    private void Update()
    {
        if (!OnlineClient.Instance)
            return;

        switch(OnlineClient.Instance.ConnectionStatus)
        {
            case ConnectionStatus.IN_LOBBY:
                PrintMetadata();
                break;
            case ConnectionStatus.CONNECTED:
                infoText.text = "You are connected to server. Trying to join lobby ...";
                break;
            case ConnectionStatus.CONNECTION_DECLINED:
            case ConnectionStatus.LOBBY_NOT_FOUND:
                infoText.text = "The server refused the connection.";
                break;
            case ConnectionStatus.UNCONNECTED:
                infoText.text = "Unable to connect to server.";
                break;
            case ConnectionStatus.ATTEMPT_CONNECTION:
                infoText.text = "No connection to server. Trying to connect ...";
                break;
        }
    }

    private void PrintMetadata()
    {
        infoText.text = "Lobby ID: " + OnlineClient.Instance.LobbyId.FullId;
        infoText.text += "\nConnected players: " + OnlineClient.Instance.PlayerCount;
        if (OnlineClient.Instance.SpectatorCount > 0)
        {
            infoText.text += "\nSpectators: " + OnlineClient.Instance.SpectatorCount;
        }
    }
}
