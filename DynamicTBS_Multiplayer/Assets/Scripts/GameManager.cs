using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startMenuCanvas;
    [SerializeField] private GameObject onlineMenuCanvas;
    [SerializeField] private GameObject draftCanvas;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject creditsCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    private List<GameObject> canvasList = new List<GameObject>();

    public static GameType gameType = GameType.local;
    private static bool hasGameStarted;

    public delegate void RecordStart();
    public static event RecordStart OnStartRecording;

    public GameObject GetGameplayCanvas() 
    {
        return gameplayCanvas;
    }

    public static void StartRecording()
    {
        if (OnStartRecording != null)
            OnStartRecording();
    }
    
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
        canvasList.Add(gameOverCanvas);
    }
    
    #region MenusRegion

    public void GoToStartMenu()
    {
        HandleMenus(startMenuCanvas);
    }

    public void GoToLocalGame()
    {
        HandleMenus(draftCanvas);
        StartRecording();
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

    private void GoToGameOverScreen(PlayerType winner)
    {
        HandleMenus(gameOverCanvas);
        Text gameOverText = gameOverCanvas.GetComponentInChildren<Text>();
        gameOverText.text = "Player " + winner.ToString() + " has won.";
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
    
    public static bool HasGameStarted()
    {
        return hasGameStarted;
    }

    private static void SetGameStarted()
    {
        hasGameStarted = true;
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += SetGameStarted;
        GameplayEvents.OnGameOver += GoToGameOverScreen;
        DraftEvents.OnEndDraft += GoToGameplayScreen;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= SetGameStarted;
        GameplayEvents.OnGameOver -= GoToGameOverScreen;
        DraftEvents.OnEndDraft -= GoToGameplayScreen;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}