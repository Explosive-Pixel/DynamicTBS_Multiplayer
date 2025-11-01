using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class LocalizationMenuHandler : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;

    private const string LanguageKey = "selectedLanguage";

    void OnEnable()
    {
        LocalizationSettings.InitializationOperation.Completed += InitComplete;
    }

    void OnDisable()
    {
        LocalizationSettings.InitializationOperation.Completed -= InitComplete;
    }

    private void InitComplete(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<LocalizationSettings> handle)
    {
        if (PlayerPrefs.HasKey(LanguageKey))
        {
            int savedLocaleIndex = PlayerPrefs.GetInt(LanguageKey);
            SetLanguage(savedLocaleIndex);
        }
        else
        {
            SetEnglish();
        }
    }

    public void SetLanguage()
    {
        if (fullscreenToggle.isOn)
            SetGerman();
        else
            SetEnglish();
    }

    private void SetEnglish()
    {
        SetLanguage("en");
    }

    private void SetGerman()
    {
        SetLanguage("de");
    }

    private void SetLanguage(string languageCode) {
        for(int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++) {
            var language = LocalizationSettings.AvailableLocales.Locales[i];
            if(language.Identifier.Code == languageCode) {
                SetLanguage(i);
            }
        }
    }

    public void SetLanguage(int localeIndex)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
        fullscreenToggle.isOn = LocalizationSettings.SelectedLocale.Identifier.Code == "de";

        PlayerPrefs.SetInt(LanguageKey, localeIndex);
        PlayerPrefs.Save();
    }
}
