using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoScreenHandler : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text infoText;

    private void Update()
    {
        switch (Client.ConnectionStatus)
        {
            case ConnectionStatus.CONNECTED:
                infoText.text = "You are connected to server. Loading setup menu ...";
                break;
            /*case ConnectionStatus.CONNECTION_DECLINED:
                infoText.text = "The server refused the connection since there are already two players in the lobby.";
                break;
            case ConnectionStatus.LOBBY_NOT_FOUND:
                infoText.text = "The server refused the connection since the requested lobby can't be found.";
                break;*/
            case ConnectionStatus.UNCONNECTED:
                infoText.text = "Unable to connect to server.";
                break;
            case ConnectionStatus.ATTEMPT_CONNECTION:
                infoText.text = "Trying to connect to server ...";
                break;
        }
    }
}
