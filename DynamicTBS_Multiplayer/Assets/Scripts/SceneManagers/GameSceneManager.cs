using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject draftCanvas;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    private List<GameObject> canvasList = new List<GameObject>();

    public GameObject GetGameplayCanvas()
    {
        return gameplayCanvas;
    }

    private void Awake()
    {
        SubscribeEvents();
        SetCanvasList();
        
    }

    private void Start()
    {
        GoToDraftScreen();
    }

    private void SetCanvasList()
    {
        canvasList.Add(draftCanvas);
        canvasList.Add(gameplayCanvas);
        canvasList.Add(gameOverCanvas);
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

    #region ScreenChangeRegion
    public void GoToDraftScreen()
    {
        HandleMenus(draftCanvas);
        DraftEvents.StartDraft();
        Debug.Log("StartDraft.");
    }

    private void GoToGameplayScreen()
    {
        HandleMenus(gameplayCanvas);
    }

    private void GoToGameOverScreen(PlayerType? winner)
    {
        HandleMenus(gameOverCanvas);
        Text gameOverText = gameOverCanvas.GetComponentInChildren<Text>();
        if (winner != null)
        {
            gameOverText.text = "Player " + winner.ToString() + " has won.";
        }
        else
        {
            gameOverText.text = "No player has won the game.";
        }
    }
    #endregion

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        DraftEvents.OnEndDraft += GoToGameplayScreen;
        GameplayEvents.OnGameOver += GoToGameOverScreen;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnEndDraft -= GoToGameplayScreen;
        GameplayEvents.OnGameOver -= GoToGameOverScreen;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion
}