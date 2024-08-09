using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineMenuSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject onlineMenuCanvas;
    [SerializeField] private GameObject onlineLoadingScreenCanvas;

    private void Awake()
    {
        GameEvents.OnGameIsLoading += ToggleOnlineLoadingScreen;
        onlineLoadingScreenCanvas.SetActive(false);

        //GoToOnlineMenu();
    }

    private void ToggleOnlineLoadingScreen(bool isLoading)
    {
        onlineLoadingScreenCanvas.SetActive(isLoading);
        if (!isLoading)
        {
            Destroy(onlineLoadingScreenCanvas);
            Destroy(this.gameObject);
        }
    }

    public void GoToOnlineMenu()
    {
        AudioEvents.PressingButton();
        onlineMenuCanvas.SetActive(true);
    }

    private void OnDestroy()
    {
        GameEvents.OnGameIsLoading -= ToggleOnlineLoadingScreen;
    }
}