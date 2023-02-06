using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineMenuSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject onlineMenuCanvas;
    [SerializeField] private GameObject onlineClientCanvas;
    [SerializeField] private GameObject onlineLoadingScreenCanvas;

    private List<GameObject> canvasList = new List<GameObject>();

    private void Awake()
    {
        GameEvents.OnGameIsLoading += ToggleOnlineLoadingScreen;
        SetCanvasList();
        GoToOnlineMenu();

        onlineLoadingScreenCanvas.SetActive(false);
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(onlineLoadingScreenCanvas);
    }

    private void SetCanvasList()
    {
        canvasList.Add(onlineMenuCanvas);
        canvasList.Add(onlineClientCanvas);
    }

    private void HandleMenus(GameObject menuCanvas)
    {
        foreach (GameObject gameObject in canvasList)
        {
            if (gameObject == menuCanvas)
                menuCanvas.SetActive(true);
            else
                gameObject.SetActive(false);
        }
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
        HandleMenus(onlineMenuCanvas);
        AudioEvents.PressingButton();
    }

    private void OnDestroy()
    {
        GameEvents.OnGameIsLoading -= ToggleOnlineLoadingScreen;
    }
}