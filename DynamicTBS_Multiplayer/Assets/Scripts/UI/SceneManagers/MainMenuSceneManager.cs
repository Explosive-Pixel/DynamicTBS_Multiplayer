using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject playOptionsMenu;
    [SerializeField] private GameObject infoOptions;

    private void Start()
    {
        AudioEvents.EnteringMainMenu();
    }

    public void TogglePlayOptionsMenu()
    {
        if (playOptionsMenu.activeSelf == true)
            playOptionsMenu.SetActive(false);
        else
            playOptionsMenu.SetActive(true);

        AudioEvents.PressingButton();
    }

    public void ToggleInfoOptions()
    {
        if (infoOptions.activeSelf == true)
            infoOptions.SetActive(false);
        else
            infoOptions.SetActive(true);

        AudioEvents.PressingButton();
    }

    /* public void SwitchToLocalGameSetup()
     {
         GameManager.GameType = GameType.LOCAL;
         SceneChangeManager.Instance.LoadScene(Scene.OFFLINE_MENU);
     }*/
}