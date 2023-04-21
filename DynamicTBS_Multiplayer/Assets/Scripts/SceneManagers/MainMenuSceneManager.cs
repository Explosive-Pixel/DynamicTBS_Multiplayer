using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject startMenuCanvas;
    [SerializeField] private GameObject mapMenu;
    [SerializeField] private GameObject infoOptions;

    private void Awake()
    {
        startMenuCanvas.SetActive(true);
    }

    private void Start()
    {
        AudioEvents.EnteringMainMenu();
    }

    public void ToggleMapMenu()
    {
        if (mapMenu.activeSelf == true)
            mapMenu.SetActive(false);
        else
            mapMenu.SetActive(true);

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
}