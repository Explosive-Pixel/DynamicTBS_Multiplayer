using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SetupOfflineGameHandler : MonoBehaviour
{
    [SerializeField] private Button startGameButton;

    private List<ISetupHandler> gameSetup = new();
    private bool SetupCompleted { get { return !gameSetup.Exists(gameSetup => !gameSetup.SetupCompleted); } }

    private bool started;

    private void Awake()
    {
        started = false;

        GameManager.GameType = GameType.LOCAL;
        gameSetup = gameObject.GetComponentsInChildren<ISetupHandler>(true).ToList();

        startGameButton.onClick.AddListener(() => StartGame());
    }

    private void Update()
    {
        startGameButton.interactable = SetupCompleted && !started;
    }

    private void StartGame()
    {
        GameEvents.StartGame();
        started = true;
    }
}
