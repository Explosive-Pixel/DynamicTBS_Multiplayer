using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsSceneManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}