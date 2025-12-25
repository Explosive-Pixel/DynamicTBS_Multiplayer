using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class OnlineMetadata : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject multiplayerMetadata;
    [SerializeField] private Text infoText;

    [Header("Localized Strings")]
    [SerializeField] private LocalizedString connectedText;
    [SerializeField] private LocalizedString unconnectedText;
    [SerializeField] private LocalizedString attemptConnectionText;

    [Header("Localized Smart Strings (with placeholders)")]
    [SerializeField] private LocalizedString lobbyIdText;
    [SerializeField] private LocalizedString connectedPlayersText;
    [SerializeField] private LocalizedString spectatorsText;

    void Awake()
    {
        infoText.text = "";
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale _)
    {
        // Aktualisiere Text bei Sprachwechsel
        UpdateText();
    }

    private void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (Client.InLobby)
        {
            PrintMetadata();
            return;
        }

        switch (Client.ConnectionStatus)
        {
            case ConnectionStatus.CONNECTED:
                SetInfoText(connectedText);
                break;

            case ConnectionStatus.UNCONNECTED:
                SetInfoText(unconnectedText);
                break;

            case ConnectionStatus.ATTEMPT_CONNECTION:
                SetInfoText(attemptConnectionText);
                break;
        }
    }

    private void PrintMetadata()
    {
        if (!Client.InLobby)
            return;

        string lobbyId = Client.CurrentLobby.LobbyId.FullId;
        int players = Client.CurrentLobby.PlayerCount;
        int spectators = Client.CurrentLobby.SpectatorCount;

        SetInfoText(lobbyIdText, false, new { id = lobbyId });
        SetInfoText(connectedPlayersText, true, new { players });
        if (spectators > 0)
        {
            SetInfoText(spectatorsText, true, new { spectators });
        }
    }

    private void SetInfoText(LocalizedString localizedString, bool append = false)
    {
        localizedString.GetLocalizedStringAsync().Completed += op =>
        {
            SetInfoText(op.Result, append);
        };
    }

    private void SetInfoText(LocalizedString localizedString, bool append, params object[] arguments)
    {
        localizedString.GetLocalizedStringAsync(arguments).Completed += op =>
        {
            SetInfoText(op.Result, append);
        };
    }

    private void SetInfoText(string text, bool append)
    {
        if (infoText)
        {
            if (append)
            {
                infoText.text += "\n" + text;
            }
            else
            {
                infoText.text = text;
            }
        }
    }
}
