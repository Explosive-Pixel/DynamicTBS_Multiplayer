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
            case ConnectionStatus.CONNECTED:
                currentString = connectedText;
                break;
            case ConnectionStatus.UNCONNECTED:
                currentString = unconnectedText;
                break;
            case ConnectionStatus.ATTEMPT_CONNECTION:
                currentString = attemptConnectionText;
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

