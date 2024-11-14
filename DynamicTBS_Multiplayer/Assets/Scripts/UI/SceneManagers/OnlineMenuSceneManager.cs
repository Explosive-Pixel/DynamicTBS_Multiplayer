using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineMenuSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject onlineLoadingScreenCanvas;

    private void Awake()
    {
        GameEvents.OnGameIsLoading += ToggleOnlineLoadingScreen;
        onlineLoadingScreenCanvas.SetActive(false);
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

    private void OnDestroy()
    {
        GameEvents.OnGameIsLoading -= ToggleOnlineLoadingScreen;
    }
}