using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject draftCanvas;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    [SerializeField] private Button playAgainButton;

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

    public void PlayAgain()
    {
        if (GameManager.gameType == GameType.local)
        {
            GameEvents.StartGame();
        }
        else
        {
            //TODO: Send message to Server to Switch Admin player;
            GameplayEvents.RestartGameplay();
            foreach (GameObject canvas in canvasList) 
            {
                canvas.SetActive(false);
            }
        }
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
    private void GoToDraftScreen()
    {
        HandleMenus(draftCanvas);
        DraftEvents.StartDraft();
    }

    private void GoToGameplayScreen()
    {
        HandleMenus(gameplayCanvas);
    }

    private void GoToGameOverScreen(PlayerType? winner)
    {
        HandleMenus(gameOverCanvas);
        Text gameOverText = gameOverCanvas.GetComponentInChildren<Text>();
        Color backgroundColor = Color.gray;
        if (winner != null)
        {
            gameOverText.text = "Player " + winner.ToString() + " has won.";
            if (winner == PlayerType.pink)
            {
                backgroundColor = new Color(1f, 0.18f, 0.8f, 1);
            }
            else
            {
                backgroundColor = new Color(0.224f, 0.53f, 0.961f, 1);
            }
        }
        else
        {
            gameOverText.text = "No player has won the game.";
        }
        gameOverCanvas.GetComponent<Image>().color = backgroundColor;
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