using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startMenuCanvas;
    [SerializeField] private GameObject onlineMenuCanvas;
    [SerializeField] private GameObject draftCanvas;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject creditsCanvas;

    private List<GameObject> canvasList = new List<GameObject>();

    private bool hasGameStarted;
    
    private void Awake()
    {
        SpriteManager.LoadSprites();
        hasGameStarted = false;
        SubscribeEvents();
        
        SetCanvasList();
        HandleMenus(startMenuCanvas);
    }

    private void SetCanvasList()
    {
        canvasList.Add(startMenuCanvas);
        canvasList.Add(onlineMenuCanvas);
        canvasList.Add(draftCanvas);
        canvasList.Add(gameplayCanvas);
        canvasList.Add(tutorialCanvas);
        canvasList.Add(creditsCanvas);
    }
    
    #region MenusRegion

    public void GoToStartMenu()
    {
        HandleMenus(startMenuCanvas);
    }

    public void GoToLocalGame()
    {
        HandleMenus(draftCanvas);
    }

    public void GoToOnlineMenu()
    {
        HandleMenus(onlineMenuCanvas);
    }

    public void GotToDraftScreen()
    {
        HandleMenus(draftCanvas);
    }

    private void GoToGameplayScreen()
    {
        HandleMenus(gameplayCanvas);
    }

    public void GoToTutorialScreen()
    {
        HandleMenus(tutorialCanvas);
    }

    public void GoToCreditsScreen()
    {
        HandleMenus(creditsCanvas);
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

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion
    
    public bool HasGameStarted()
    {
        return hasGameStarted;
    }

    private void SetGameStarted()
    {
        hasGameStarted = true;
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += SetGameStarted;
        DraftEvents.OnEndDraft += GoToGameplayScreen;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= SetGameStarted;
        DraftEvents.OnEndDraft -= GoToGameplayScreen;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}