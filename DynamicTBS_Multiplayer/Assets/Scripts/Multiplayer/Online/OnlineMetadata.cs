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
        DontDestroyOnLoad(multiplayerMetadata);

        infoText.text = "";
    }

    private void Update()
    {
        if (!OnlineClient.Instance)
            return;

        switch(OnlineClient.Instance.ConnectionStatus)
        {
            case ConnectionStatus.CONNECTED:
                PrintMetadata();
                break;
            case ConnectionStatus.CONNECTION_DECLINED:
            case ConnectionStatus.LOBBY_NOT_FOUND:
                infoText.text = "Unable to connect to server.";
                break;
            case ConnectionStatus.UNCONNECTED:
                infoText.text = "Unable to reconnect to server.";
                break;
            case ConnectionStatus.ATTEMPT_CONNECTION:
                infoText.text = "Lost connection to server. Trying to reconnect ...";
                break;
        }
    }

    private void PrintMetadata()
    {
        infoText.text = "Connected players: " + OnlineClient.Instance.PlayerCount;
        if (OnlineClient.Instance.SpectatorCount > 0)
        {
            infoText.text += "\nSpectators: " + OnlineClient.Instance.SpectatorCount;
        }
    }
}
