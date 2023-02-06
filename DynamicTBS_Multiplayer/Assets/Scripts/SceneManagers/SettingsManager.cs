using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    private GameObject settingsObject;

    [SerializeField] private GameObject toggleObjects;

    [SerializeField] private AudioMixer audioMixer;
    public static float currentMainVolume;
    public static float currentMusicVolume;
    public static float currentAtmoVolume;
    public static float currentFXVolume;
    public static int currentFullscreenSetting;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
        GameEvents.GameHasBooted();
    }

    private void Start()
    {
        settingsObject = this.gameObject;
        DontDestroyOnLoad(settingsObject);
        toggleObjects.SetActive(false);
    }

    public void SetMainVolume(float volume)
    {
        audioMixer.SetFloat("MainVolume", volume);
        currentMainVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
        currentMusicVolume = volume;
    }

    public void SetAtmoVolume(float volume)
    {
        audioMixer.SetFloat("AtmoVolume", volume);
        currentAtmoVolume = volume;
    }

    public void SetFXVolume(float volume)
    {
        audioMixer.SetFloat("FXVolume", volume);
        currentFXVolume = volume;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        if (isFullscreen)
            currentFullscreenSetting = 1;
        else
            currentFullscreenSetting = 0;
    }

    public void ToggleSettings()
    {
        if (toggleObjects.activeSelf == false)
            toggleObjects.SetActive(true);
        else
            toggleObjects.SetActive(false);
        AudioEvents.PressingButton();
    }
}