using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject startMenuCanvas;
    [SerializeField] private GameObject playOptions;
    [SerializeField] private GameObject infoOptions;

    private void Awake()
    {
        startMenuCanvas.SetActive(true);
    }

    private void Start()
    {
        AudioEvents.EnteringMainMenu();
    }

    public void TogglePlayOptions()
    {
        if (playOptions.activeSelf == true)
            playOptions.SetActive(false);
        else
            playOptions.SetActive(true);
    }

    public void ToggleInfoOptions()
    {
        if (infoOptions.activeSelf == true)
            infoOptions.SetActive(false);
        else
            infoOptions.SetActive(true);
    }
}