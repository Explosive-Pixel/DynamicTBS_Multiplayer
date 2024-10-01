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
        switch (Client.ConnectionStatus)
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
        if (!Client.InLobby)
            return;

        infoText.text = "Lobby ID: " + Client.CurrentLobby.LobbyId.FullId;
        infoText.text += "\nConnected players: " + Client.CurrentLobby.PlayerCount;
        if (Client.CurrentLobby.SpectatorCount > 0)
        {
            infoText.text += "\nSpectators: " + Client.CurrentLobby.SpectatorCount;
        }
    }
}
