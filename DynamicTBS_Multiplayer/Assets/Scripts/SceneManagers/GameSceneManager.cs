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

    public delegate void GamePhase();
    public static event GamePhase OnGameStart;

    public static void StartGame()
    {
        if (OnGameStart != null)
            OnGameStart();
    }

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
    private void GoToDraftScreen()
    {
        Debug.Log("SCENE: Going to draft Screen");
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
        OnGameStart += GoToDraftScreen;
        DraftEvents.OnEndDraft += GoToGameplayScreen;
        GameplayEvents.OnGameOver += GoToGameOverScreen;
    }

    private void UnsubscribeEvents()
    {
        OnGameStart -= GoToDraftScreen;
        DraftEvents.OnEndDraft -= GoToGameplayScreen;
        GameplayEvents.OnGameOver -= GoToGameOverScreen;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion
}