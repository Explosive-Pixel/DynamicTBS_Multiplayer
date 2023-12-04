using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject draftCanvas;
    [SerializeField] private GameObject draftObjectsObject;
    [SerializeField] private GameObject gameplayCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject gameplayObjectsObject;
    [SerializeField] private GameObject pauseCanvas;

    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button backToLobbyButton;
    [SerializeField] private List<Button> backToMainMenuButtons;

    [SerializeField] private int goToGameOverCanvasDelay;
    [SerializeField] private int goToGameplayScreenDelay;

    private readonly List<GameObject> canvasList = new();

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

        GameplayEvents.RestartGameplay();
        SceneChangeManager.Instance.LoadScene(Scene.GAME_SETUP);
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
        draftObjectsObject.SetActive(true);
    }

    private void GoToGameplayScreen(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.DRAFT)
        {
            draftObjectsObject.SetActive(false);

            HandleMenus(gameplayCanvas);
            gameplayObjectsObject.SetActive(true);
        }
    }

    private void GoToGameOverScreen(PlayerType? winner, GameOverCondition endGameCondition)
    {
        StartCoroutine(DelayGoToGameOverScreen());
    }

    private IEnumerator DelayGoToGameOverScreen()
    {
        yield return new WaitForSeconds(goToGameOverCanvasDelay);
        HandleMenus(gameOverCanvas);
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