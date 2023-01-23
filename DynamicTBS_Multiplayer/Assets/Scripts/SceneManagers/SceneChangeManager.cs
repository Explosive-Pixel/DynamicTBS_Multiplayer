using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    // Scene order is as follows:
    // 0: Main Menu
    // 1: Online Menu
    // 2: Game Scene (Draft/Placement/Fight/GameOver)
    // 3: Tutorial Scene
    // 4: Lore Scene
    // 5: Credits Scene
    
    // TODO: Put saving and loading PlayerPrefs and the Quit-Option in separate class which doesn't destroy on load.
    private void Awake()
    {
        SubscribeEvents();
    }

    public void LoadMainMenuScene()
    {
        LoadSceneOnButtonPress(0);
    }

    public void LoadOnlineMenuScene()
    {
        LoadSceneOnButtonPress(1);
    }

    public void LoadGameScene()
    {
        LoadSceneOnButtonPress(2);
    }

    public void LoadTutorialScene()
    {
        LoadSceneOnButtonPress(3);
    }

    public void LoadLoreScene()
    {
        LoadSceneOnButtonPress(4);
    }

    public void LoadCreditsScene()
    {
        LoadSceneOnButtonPress(5);
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
        if (PlayerPrefs.HasKey("FullscreenSetting"))
            SettingsManager.currentFullscreenSetting = PlayerPrefs.GetInt("FullscreenSetting");
    }

    private void LoadSceneOnButtonPress(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber, LoadSceneMode.Single);

        // Shut down Server and Client if it is not the online menu or game scene
        if(sceneNumber != 1 && sceneNumber != 2)
        {
            DestroyManually("OnlineGameManager");
            DestroyManually("OnlineClientCanvas");
            DestroyManually("OnlineMetadataCanvas");
            DestroyManually("OnlineMenuSceneManager");
            DestroyManually("OnlineLoadingScreenCanvas");
        }
    }

    private void DestroyManually(string gameObjectName)
    {
        GameObject go = GameObject.Find(gameObjectName);
        if(go)
        {
            Destroy(go);
        }
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameEvents.OnGameStart += LoadGameScene;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGameStart -= LoadGameScene;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion
}