using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;

public class InfoScreenHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;

    [Header("Localized Strings")]
    [SerializeField] private LocalizedString connectedText;
    [SerializeField] private LocalizedString unconnectedText;
    [SerializeField] private LocalizedString attemptConnectionText;
    [SerializeField] private LocalizedString outdatedText;

    private void Awake()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale _)
    {
        UpdateText();
    }

    private void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (infoText == null)
            return;

        LocalizedString currentString = null;

        switch (Client.ConnectionStatus)
        {
            case ConnectionState.CONNECTED:
                currentString = connectedText;
                break;
            case ConnectionState.DICONNECTED:
            case ConnectionState.DEAD:
                currentString = unconnectedText;
                break;
            case ConnectionState.CONNECTING:
            case ConnectionState.RECONNECTING:
                currentString = attemptConnectionText;
                break;
            case ConnectionState.OUTDATED:
                currentString = outdatedText;
                break;
        }

        if (currentString != null)
        {
            var handle = currentString.GetLocalizedStringAsync();
            handle.Completed += op =>
            {
                infoText.text = op.Result;
            };
        }
    }
}

