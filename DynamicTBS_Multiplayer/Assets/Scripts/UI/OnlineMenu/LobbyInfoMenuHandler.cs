using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfoMenuHandler : MonoBehaviour
{
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button readyButton;

    private Lobby SelectedLobby { get { return gameObject.GetComponentInChildren<LobbyDetailsHandler>().SelectedLobby; } }

    private void Awake()
    {
        readyButton.onClick.AddListener(() => SetReady());
        joinLobbyButton.onClick.AddListener(() => JoinLobby());
    }

    private void Update()
    {
        joinLobbyButton.gameObject.SetActive(!Client.InLobby && SelectedLobby != null);
        readyButton.interactable = !Client.IsReady;
        readyButton.gameObject.SetActive(Client.InLobby);
    }

    private void SetReady()
    {
        Client.IsReady = true;
        Client.SendToServer(new WSMsgSetReady());
    }

    private void JoinLobby()
    {
        if (SelectedLobby == null)
            return;

        Client.JoinLobby(SelectedLobby.LobbyId.FullId, ClientType.PLAYER);
    }
}
