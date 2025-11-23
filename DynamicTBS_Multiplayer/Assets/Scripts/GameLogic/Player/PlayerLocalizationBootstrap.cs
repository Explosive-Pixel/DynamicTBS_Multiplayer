using UnityEngine;
using UnityEngine.Localization;

public class PlayerLocalizationBootstrap : MonoBehaviour
{
    [SerializeField] private LocalizedString pinkNameLocalized;
    [SerializeField] private LocalizedString blueNameLocalized;

    private void Start()
    {
        pinkNameLocalized.StringChanged += (value) =>
        {
            PlayerSetup.SetLocalizedDefault(PlayerType.pink, value);
        };

        blueNameLocalized.StringChanged += (value) =>
        {
            PlayerSetup.SetLocalizedDefault(PlayerType.blue, value);
        };

        // Initial load now
        pinkNameLocalized.RefreshString();
        blueNameLocalized.RefreshString();
    }
}

