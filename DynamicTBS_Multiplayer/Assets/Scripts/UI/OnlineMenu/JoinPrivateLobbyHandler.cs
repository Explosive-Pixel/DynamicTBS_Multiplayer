using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static WSMsgServerNotification;

public class JoinPrivateLobbyHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField fullLobbyName;

    [SerializeField] private Button joinAsSpectatorButton;
    [SerializeField] private Button joinLobbyButton;

    [SerializeField] private TMP_Text lobbyInfoText;

    [SerializeField] private GameObject nameSetup;

    private ISetupHandler nameSetupHandler;

    private bool SetupCompleted { get { return IsValidLobbyID() && nameSetupHandler.SetupCompleted; } }

    private void Awake()
    {
        nameSetupHandler = nameSetup.GetComponent<ISetupHandler>();

        ResetLobbyInfoText();
        fullLobbyName.onValueChanged.AddListener(value => ResetLobbyInfoText());

        joinAsSpectatorButton.onClick.AddListener(() => JoinLobby(ClientType.SPECTATOR));
        joinLobbyButton.onClick.AddListener(() => JoinLobby(ClientType.PLAYER));
    }

    private void OnEnable()
    {
        MessageReceiver.OnWSMessageReceive += UpdateLobbyInfo;
    }

    private void Update()
    {
        joinAsSpectatorButton.interactable = SetupCompleted;
        joinLobbyButton.interactable = SetupCompleted;
    }

    private void UpdateLobbyInfo(WSMessage msg)
    {
        if (msg.code == WSMessageCode.WSMsgServerNotificationCode)
        {
            switch (((WSMsgServerNotification)msg).serverNotification)
            {
                case ServerNotification.LOBBY_NOT_FOUND:
                    lobbyInfoText.text = "Lobby not found.";
                    break;
                case ServerNotification.CONNECTION_FORBIDDEN_FULL_LOBBY:
                    lobbyInfoText.text = "Lobby is already full.";
                    break;
            }
        }
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

    private void ResetLobbyInfoText()
    {
        lobbyInfoText.text = "";
    }

    private void OnDisable()
    {
        MessageReceiver.OnWSMessageReceive -= UpdateLobbyInfo;
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
