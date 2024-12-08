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
        if (Client.InLobby)
        {
            PrintMetadata();
            return;
        }

        switch (Client.ConnectionStatus)
        {
            case ConnectionStatus.CONNECTED:
                infoText.text = "You are connected to server.";
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
