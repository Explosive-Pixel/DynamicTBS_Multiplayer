using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinPrivateLobbyHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clientName;

    [SerializeField] private TextMeshProUGUI fullLobbyName;

    [SerializeField] private Button joinAsSpectatorButton;
    [SerializeField] private Button joinLobbyButton;

    private string ClientName { get { return clientName.text.Trim(); } }

    private bool Completed { get { return IsPresent(ClientName) && IsValidLobbyID(); } }

    private void Awake()
    {
        joinAsSpectatorButton.onClick.AddListener(() => JoinLobby(ClientType.SPECTATOR));
        joinLobbyButton.onClick.AddListener(() => JoinLobby(ClientType.PLAYER));
    }

    private void Update()
    {
        joinAsSpectatorButton.interactable = Completed;
        joinLobbyButton.interactable = Completed;
    }

    private void JoinLobby(ClientType role)
    {
        if (Completed)
        {
            Client.UserName = ClientName;

            Client.JoinLobby(fullLobbyName.text.Trim(), role);
        }
    }

    private bool IsValidLobbyID()
    {
        return fullLobbyName.text.Trim().Length > 0 && LobbyId.FromFullId(fullLobbyName.text) != null;
    }

    private bool IsPresent(string text)
    {
        return text != null && text.Length > 0;
    }
}
