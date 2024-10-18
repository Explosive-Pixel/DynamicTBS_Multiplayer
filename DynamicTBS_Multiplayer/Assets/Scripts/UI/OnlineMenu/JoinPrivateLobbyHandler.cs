using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinPrivateLobbyHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField fullLobbyName;

    [SerializeField] private Button joinAsSpectatorButton;
    [SerializeField] private Button joinLobbyButton;

    private bool SetupCompleted { get { return IsValidLobbyID(); } }

    private void Awake()
    {
        joinAsSpectatorButton.onClick.AddListener(() => JoinLobby(ClientType.SPECTATOR));
        joinLobbyButton.onClick.AddListener(() => JoinLobby(ClientType.PLAYER));
    }

    private void Update()
    {
        joinAsSpectatorButton.interactable = SetupCompleted;
        joinLobbyButton.interactable = SetupCompleted;
    }

    private void JoinLobby(ClientType role)
    {
        if (SetupCompleted)
        {
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
