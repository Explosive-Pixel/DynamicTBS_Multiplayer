using UnityEngine;
using UnityEngine.Localization;

public class PlayerLocalizationBootstrap : MonoBehaviour
{
    [SerializeField] private LocalizedString pinkNameLocalized;
    [SerializeField] private LocalizedString blueNameLocalized;

    private void Start()
    {
        pinkNameLocalized.StringChanged += UpdatePinkValue;
        blueNameLocalized.StringChanged += UpdateBlueValue;

        // Initial load now
        pinkNameLocalized.RefreshString();
        blueNameLocalized.RefreshString();
    }

    private void UpdatePinkValue(string value)
    {
        PlayerSetup.SetLocalizedDefault(PlayerType.pink, value);
    }

    private void UpdateBlueValue(string value)
    {
        PlayerSetup.SetLocalizedDefault(PlayerType.blue, value);
    }

    private void OnDestroy()
    {
        pinkNameLocalized.StringChanged -= UpdatePinkValue;
        blueNameLocalized.StringChanged -= UpdateBlueValue;
    }
}

