using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyDetailsHandler : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text lobbyName;
    [SerializeField] private TMPro.TMP_Text lobbyType;

    [SerializeField] private TMPro.TMP_Text bluePlayerName;
    [SerializeField] private TMPro.TMP_Text pinkPlayerName;
    [SerializeField] private GameObject waitingForBluePlayer;
    [SerializeField] private GameObject waitingForPinkPlayer;

    [SerializeField] private TMPro.TMP_Text spectators;
    [SerializeField] private Button joinAsSpectatorButton;

    private void Update()
    {
        if (!Client.InLobby)
            return;

        lobbyName.text = Client.CurrentLobby.LobbyId.FullId;

        HandlePlayerText(PlayerType.blue);
        HandlePlayerText(PlayerType.pink);

        spectators.text = Client.CurrentLobby.SpectatorCount.ToString();
    }

    private void HandlePlayerText(PlayerType side)
    {
        ClientInfo player = Client.CurrentLobby.GetPlayer(side);

        if (player == null)
        {
            if (side == PlayerType.blue)
            {
                bluePlayerName.gameObject.SetActive(false);
                waitingForBluePlayer.SetActive(true);
            }
            else
            {
                pinkPlayerName.gameObject.SetActive(false);
                waitingForPinkPlayer.SetActive(true);
            }
        }
        else
        {
            if (side == PlayerType.blue)
            {
                bluePlayerName.gameObject.SetActive(true);
                bluePlayerName.text = player.name;
                waitingForBluePlayer.SetActive(false);
            }
            else
            {
                pinkPlayerName.gameObject.SetActive(true);
                pinkPlayerName.text = player.name;
                waitingForPinkPlayer.SetActive(false);
            }
        }

    }
}
