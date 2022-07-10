using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startMenuCanvas;
    [SerializeField] private GameObject onlineMenuCanvas;
    [SerializeField] private GameObject draftCanvas;
    [SerializeField] private GameObject gameplayCanvas;

    private List<GameObject> canvasList = new List<GameObject>();

    private bool hasGameStarted;
    
    private void Awake()
    {
        SpriteManager.LoadSprites();
        hasGameStarted = false;
        SubscribeEvents();
    }

    
    
    #region MenusRegion

    public void GoToStartMenu()
    {
        startMenuCanvas.SetActive(true);
        onlineMenuCanvas.SetActive(false);
        draftCanvas.SetActive(false);
        gameplayCanvas.SetActive(false);
    }

    public void GoToLocalGame()
    {
        
    }

    public void GoToOnlineMenu()
    {
        startMenuCanvas.SetActive(false);
        onlineMenuCanvas.SetActive(true);
        draftCanvas.SetActive(false);
        gameplayCanvas.SetActive(false);
    }

    public void GotToDraftScreen()
    {
        startMenuCanvas.SetActive(false);
        onlineMenuCanvas.SetActive(false);
        draftCanvas.SetActive(true);
        gameplayCanvas.SetActive(false);
    }

    private void GoToGameplayScreen()
    {
        startMenuCanvas.SetActive(false);
        onlineMenuCanvas.SetActive(false);
        draftCanvas.SetActive(false);
        gameplayCanvas.SetActive(true);
    }

    public void GoToTutorialScreen()
    {
        
    }

    public void GoToCreditScreen()
    {
        
    }

    private void HandleMenus(GameObject menuCanvas)
    {
        
    }

    #endregion
    
    public bool HasGameStarted()
    {
        return hasGameStarted;
    }

    private void SetGameStarted()
    {
        hasGameStarted = true;
        Debug.Log("Game hast started: " + hasGameStarted);
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += SetGameStarted;
        DraftEvents.OnEndDraft += GoToGameplayScreen;
        Debug.Log("GameManager: Subscribed to events.");
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= SetGameStarted;
        DraftEvents.OnEndDraft -= GoToGameplayScreen;
        Debug.Log("Unsubscribed from events.");
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}