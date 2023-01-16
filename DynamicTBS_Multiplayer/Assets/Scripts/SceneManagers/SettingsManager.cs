using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    private GameObject settingsObject;

    [SerializeField] private GameObject toggleObjects;

    [SerializeField] private AudioMixer audioMixer;
    public static float currentVolume;
    public static int currentFullscreenSetting;

    private void Awake()
    {
        settingsObject = this.gameObject;
        DontDestroyOnLoad(settingsObject);
        toggleObjects.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
        currentVolume = volume;
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
    }
}