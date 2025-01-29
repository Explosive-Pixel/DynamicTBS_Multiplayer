using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    private GameObject settingsObject;

    [SerializeField] private GameObject toggleObjects;

    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider mainVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider atmoVolumeSlider;
    [SerializeField] private Slider fxVolumeSlider;
    [SerializeField] private Slider voiceVolumeSlider;

    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        settingsObject = this.gameObject;
        DontDestroyOnLoad(settingsObject);
        toggleObjects.SetActive(false);

        LoadSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        if (toggleObjects.activeSelf == false)
            toggleObjects.SetActive(true);
        else
            toggleObjects.SetActive(false);
        AudioEvents.PressingButton();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        fullscreenToggle.isOn = isFullscreen;
    }

    public void SetMainVolume(float volume)
    {
        audioMixer.SetFloat("MainVolume", volume);
        mainVolumeSlider.value = volume;
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        musicVolumeSlider.value = volume;
    }

    public void SetAtmoVolume(float volume)
    {
        audioMixer.SetFloat("AtmoVolume", volume);
        atmoVolumeSlider.value = volume;
    }

    public void SetFXVolume(float volume)
    {
        audioMixer.SetFloat("FXVolume", volume);
        fxVolumeSlider.value = volume;
    }

    public void SetVoiceVolume(float volume)
    {
        audioMixer.SetFloat("VoiceVolume", volume);
        voiceVolumeSlider.value = volume;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("FullscreenSetting", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("MainVolumeSetting", mainVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolumeSetting", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("AtmoVolumeSetting", atmoVolumeSlider.value);
        PlayerPrefs.SetFloat("FXVolumeSetting", fxVolumeSlider.value);
        PlayerPrefs.SetFloat("VoiceVolumeSetting", voiceVolumeSlider.value);

        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("FullscreenSetting"))
            SetFullscreen(PlayerPrefs.GetInt("FullscreenSetting") == 1);
        if (PlayerPrefs.HasKey("MainVolumeSetting"))
            SetMainVolume(PlayerPrefs.GetFloat("MainVolumeSetting"));
        if (PlayerPrefs.HasKey("MusicVolumeSetting"))
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolumeSetting"));
        if (PlayerPrefs.HasKey("AtmoVolumeSetting"))
            SetAtmoVolume(PlayerPrefs.GetFloat("AtmoVolumeSetting"));
        if (PlayerPrefs.HasKey("FXVolumeSetting"))
            SetFXVolume(PlayerPrefs.GetFloat("FXVolumeSetting"));
        if (PlayerPrefs.HasKey("VoiceVolumeSetting"))
            SetVoiceVolume(PlayerPrefs.GetFloat("VoiceVolumeSetting"));
    }

    //public void QuitGame()
    //{
    //    //SaveSettings();
    //    Application.Quit();
    //}

    private void OnDestroy()
    {
        SaveSettings();
    }
}