using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject draftCanvas;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject gameplayObjectsObject;
    [SerializeField] private GameObject pauseCanvas;

    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button backToLobbyButton;
    [SerializeField] private List<Button> backToMainMenuButtons;

    private List<GameObject> canvasList = new List<GameObject>();

    public GameObject GetGameplayCanvas()
    {
        return gameplayCanvas;
    }

    private void Awake()
    {
        SubscribeEvents();
        SetCanvasList();

        playAgainButton.gameObject.SetActive(GameManager.gameType == GameType.LOCAL);
        backToLobbyButton.gameObject.SetActive(GameManager.gameType == GameType.ONLINE);
        backToMainMenuButtons.ForEach(button => button.gameObject.SetActive(!GameManager.IsPlayer()));

        pauseCanvas.SetActive(false);
    }

    private void Start()
    {
        GoToDraftScreen();
    }

    public void PlayAgain()
    {
        AudioEvents.PressingButton();

        if (GameManager.gameType == GameType.LOCAL)
        {
            GameEvents.StartGame();
        }
        else
        {
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
            gameObject.SetActive(gameObject == menuCanvas);
        }
    }

    private void TogglePauseCanvas(bool paused)
    {
        pauseCanvas.SetActive(paused);
    }

    #region ScreenChangeRegion
    private void GoToDraftScreen()
    {
        HandleMenus(draftCanvas);
    }

    private void GoToGameplayScreen(GamePhase gamePhase)
    {
        if(gamePhase == GamePhase.DRAFT)
        {
            HandleMenus(gameplayCanvas);
            gameplayObjectsObject.SetActive(true);
        }
    }

    private void GoToGameOverScreen(PlayerType? winner, GameOverCondition endGameCondition)
    {
        HandleMenus(gameOverCanvas);
        gameplayObjectsObject.SetActive(false);
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
        gameOverText.text += "\n\n" + endGameCondition.ToText(winner);
        gameOverCanvas.GetComponentInChildren<Image>().color = backgroundColor;
    }
    #endregion

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseEnd += GoToGameplayScreen;
        GameplayEvents.OnGameOver += GoToGameOverScreen;
        GameplayEvents.OnGamePause += TogglePauseCanvas;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseEnd -= GoToGameplayScreen;
        GameplayEvents.OnGameOver -= GoToGameOverScreen;
        GameplayEvents.OnGamePause -= TogglePauseCanvas;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion
}