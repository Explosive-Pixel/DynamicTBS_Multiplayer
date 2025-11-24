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

    private void Awake()
    {
        infoText.text = "";
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDestroy()
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
                connectedText.GetLocalizedStringAsync().Completed += op =>
                {
                    SetInfoText(op.Result);
                };
                break;

            case ConnectionStatus.UNCONNECTED:
                unconnectedText.GetLocalizedStringAsync().Completed += op =>
                {
                    SetInfoText(op.Result);
                };
                break;

            case ConnectionStatus.ATTEMPT_CONNECTION:
                attemptConnectionText.GetLocalizedStringAsync().Completed += op =>
                {
                    SetInfoText(op.Result);
                };
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

        infoText.text = "";

        // Lobby ID
        lobbyIdText.GetLocalizedStringAsync(new { id = lobbyId }).Completed += op =>
        {
            SetInfoText(op.Result);
        };

        // Player count
        connectedPlayersText.GetLocalizedStringAsync(new { players }).Completed += op =>
        {
            SetInfoText("\n" + op.Result, true);
        };

        // Spectators (optional)
        if (spectators > 0)
        {
            spectatorsText.GetLocalizedStringAsync(new { spectators }).Completed += op =>
            {
                SetInfoText("\n" + op.Result, true);
            };
        }
    }

    private void SetInfoText(string text, bool append = false)
    {
        if (infoText)
        {
            if (append)
            {
                infoText.text += text;
            }
            else
            {
                infoText.text = text;
            }
        }
    }
}
