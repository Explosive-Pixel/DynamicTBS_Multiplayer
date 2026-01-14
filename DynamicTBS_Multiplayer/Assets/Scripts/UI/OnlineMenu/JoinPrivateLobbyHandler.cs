using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using static WSMsgServerNotification;

public class JoinPrivateLobbyHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField fullLobbyName;

    [SerializeField] private Toggle joinAsSpectator;
    [SerializeField] private Button joinLobbyButton;

    [SerializeField] private TMP_Text lobbyInfoText;

    [SerializeField] private GameObject nameSetup;

    [Header("Localized Strings")]
    [SerializeField] private LocalizedString lobbyNotFoundText;
    [SerializeField] private LocalizedString lobbyFullText;

    private ISetupHandler nameSetupHandler;

    // Letzter angezeigter LocalizedString
    private LocalizedString lastDisplayedMessage;

    private bool SetupCompleted => IsValidLobbyID() && nameSetupHandler.SetupCompleted;

    private void Awake()
    {
        nameSetupHandler = nameSetup.GetComponent<ISetupHandler>();

        ResetLobbyInfoText();
        fullLobbyName.onValueChanged.AddListener(value => ResetLobbyInfoText());

        joinLobbyButton.onClick.AddListener(() => JoinLobby());

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        OnDisable();
    }

    private void OnEnable()
    {
        MessageReceiver.OnWSMessageReceive += UpdateLobbyInfo;
    }

    private void OnDisable()
    {
        MessageReceiver.OnWSMessageReceive -= UpdateLobbyInfo;
    }

    private void Update()
    {
        joinLobbyButton.interactable = SetupCompleted;
    }

    private void UpdateLobbyInfo(WSMessage msg)
    {
        if (msg.code != WSMessageCode.WSMsgServerNotificationCode)
            return;

        switch (((WSMsgServerNotification)msg).serverNotification)
        {
            case ServerNotification.LOBBY_NOT_FOUND:
                SetLobbyInfoText(lobbyNotFoundText);
                break;
            case ServerNotification.CONNECTION_FORBIDDEN_FULL_LOBBY:
                SetLobbyInfoText(lobbyFullText);
                break;
        }
    }

    private void SetOutline(TMP_InputField tMP_Input, bool enabled)
    {
        tMP_Input.gameObject.GetComponent<Outline>().enabled = enabled;
    }

    private void SetLobbyInfoText(LocalizedString localizedString)
    {
        if (localizedString == null || lobbyInfoText == null)
            return;

        lastDisplayedMessage = localizedString;

        var handle = localizedString.GetLocalizedStringAsync();
        handle.Completed += op =>
        {
            lobbyInfoText.text = op.Result;
        };

        SetOutline(fullLobbyName, true);
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale _)
    {
        // Bei Sprachwechsel den letzten angezeigten LocalizedString erneut abrufen
        if (lastDisplayedMessage != null)
        {
            var handle = lastDisplayedMessage.GetLocalizedStringAsync();
            handle.Completed += op =>
            {
                lobbyInfoText.text = op.Result;
            };
        }
    }

    private void JoinLobby()
    {
        if (IsValidLobbyID())
        {
            ClientType role = joinAsSpectator.isOn ? ClientType.SPECTATOR : ClientType.PLAYER;
            Client.JoinLobby(fullLobbyName.text != null ? fullLobbyName.text.Trim() : "", role);
        }
    }

    private bool IsValidLobbyID()
    {
        return fullLobbyName.text.Trim().Length > 0 && LobbyId.FromFullId(fullLobbyName.text) != null;
    }

    private void ResetLobbyInfoText()
    {
        lobbyInfoText.text = "";
        lastDisplayedMessage = null;
        SetOutline(fullLobbyName, !IsValidLobbyID());
    }
}
