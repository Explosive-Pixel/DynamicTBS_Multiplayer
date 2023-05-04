using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public enum Scene
{
    SERVER = 0,
    MAIN_MENU = 1,
    ONLINE_MENU = 2,
    GAME = 3,
    TUTORIAL = 4,
    LORE = 5,
    CREDITS = 6
}

public class SceneChangeManager : MonoBehaviour
{
    // TODO: Put saving and loading PlayerPrefs and the Quit-Option in separate class which doesn't destroy on load.
    private void Awake()
    {
        SubscribeEvents();
    }

    public void LoadMainMenuScene()
    {
        LoadSceneOnButtonPress(Scene.MAIN_MENU);
    }

    public void LoadOnlineMenuScene()
    {
        LoadSceneOnButtonPress(Scene.ONLINE_MENU);
    }

    public void LoadGameScene()
    {
        LoadSceneOnButtonPress(Scene.GAME);
    }

    public void LoadTutorialScene()
    {
        LoadSceneOnButtonPress(Scene.TUTORIAL);
    }

    public void LoadLoreScene()
    {
        LoadSceneOnButtonPress(Scene.LORE);
    }

    public void LoadCreditsScene()
    {
        LoadSceneOnButtonPress(Scene.CREDITS);
    }

    public void QuitGame()
    {
        SaveSettings();
        Application.Quit();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MainVolumeSetting", SettingsManager.currentMainVolume);
        PlayerPrefs.SetFloat("MusicVolumeSetting", SettingsManager.currentMusicVolume);
        PlayerPrefs.SetFloat("AtmoVolumeSetting", SettingsManager.currentAtmoVolume);
        PlayerPrefs.SetFloat("FXVolumeSetting", SettingsManager.currentFXVolume);
        PlayerPrefs.SetFloat("VoiceVolumeSetting", SettingsManager.currentVoiceVolume);
        PlayerPrefs.SetInt("FullscreenSetting", SettingsManager.currentFullscreenSetting);
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MainVolumeSetting"))
            SettingsManager.currentMainVolume = PlayerPrefs.GetFloat("MainVolumeSetting");
        if (PlayerPrefs.HasKey("MusicVolumeSetting"))
            SettingsManager.currentMusicVolume = PlayerPrefs.GetFloat("MusicVolumeSetting");
        if (PlayerPrefs.HasKey("AtmoVolumeSetting"))
            SettingsManager.currentAtmoVolume = PlayerPrefs.GetFloat("AtmoVolumeSetting");
        if (PlayerPrefs.HasKey("FXVolumeSetting"))
            SettingsManager.currentFXVolume = PlayerPrefs.GetFloat("FXVolumeSetting");
        if (PlayerPrefs.HasKey("VoiceVolumeSetting"))
            SettingsManager.currentVoiceVolume = PlayerPrefs.GetFloat("VoiceVolumeSetting");
        if (PlayerPrefs.HasKey("FullscreenSetting"))
            SettingsManager.currentFullscreenSetting = PlayerPrefs.GetInt("FullscreenSetting");
    }

    private void LoadSceneOnButtonPress(Scene scene)
    {
        AudioEvents.PressingButton();

        int sceneNumber = (int)scene;
        SceneManager.LoadScene(sceneNumber, LoadSceneMode.Single);

        // Shut down Server and Client if it is not the online menu or game scene
        if(scene != Scene.ONLINE_MENU && scene != Scene.GAME)
        {
            GameObject.FindGameObjectsWithTag("DestroyInMainMenu").ToList().ForEach(go => Destroy(go));
        }
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameEvents.OnGameBoot += LoadSettings;
        GameEvents.OnGameStart += LoadGameScene;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGameStart -= LoadGameScene;
        GameEvents.OnGameBoot -= LoadSettings;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion
}