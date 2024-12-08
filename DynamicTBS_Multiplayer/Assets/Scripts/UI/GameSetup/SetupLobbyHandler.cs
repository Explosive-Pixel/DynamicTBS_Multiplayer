using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetupLobbyHandler : MonoBehaviour, ISetupHandler
{
    [SerializeField] private TMP_InputField lobbyName;
    [SerializeField] private Button privateLobbyButton;
    [SerializeField] private Button publicLobbyButton;

    public bool IsPrivateLobby { get; private set; } = false;
    public string LobbyName { get { return lobbyName.text.Trim(); } }

    public bool SetupCompleted { get { return LobbyName.Length > 0; } }

    private void Awake()
    {
        privateLobbyButton.onClick.AddListener(() => SelectPrivateLobbyButton());
        publicLobbyButton.onClick.AddListener(() => SelectPublicLobbyButton());
        SelectLobbyTypeButton(false);
    }

    private void SelectPrivateLobbyButton()
    {
        AudioEvents.PressingButton();
        SelectLobbyTypeButton(true);
    }

    private void SelectPublicLobbyButton()
    {
        AudioEvents.PressingButton();
        SelectLobbyTypeButton(false);
    }

    private void SelectLobbyTypeButton(bool privateLobby)
    {
        IsPrivateLobby = privateLobby;
        privateLobbyButton.interactable = !privateLobby;
        publicLobbyButton.interactable = privateLobby;
    }
}
