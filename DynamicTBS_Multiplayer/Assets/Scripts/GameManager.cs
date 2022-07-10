using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startMenuCanvas;
    [SerializeField] private GameObject onlineMenuCanvas;
    [SerializeField] private GameObject draftCanvas;
    [SerializeField] private GameObject gameplayCanvas;

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