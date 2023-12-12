using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LocalGameSetupCanvasHandler : MonoBehaviour
{
    [SerializeField] private Button startGameButton;

    [SerializeField] private GameSetupHandler gameSetupHandler;

    [SerializeField] private bool skipSetup = false;
    [SerializeField] private MapType defaultMap;
    [SerializeField] private TimerSetupType defaultTimer;

    private bool active = false;

    private void Awake()
    {
        if (GameManager.GameType == GameType.LOCAL)
        {
            if (skipSetup)
            {
                StartLocalGameWithDefaultSetup();
                return;
            }

            active = true;
            startGameButton.onClick.AddListener(() => StartLocalGame());
        }
    }

    private void Update()
    {
        if (!active)
            return;

        startGameButton.interactable = gameSetupHandler.AllSelected;
    }

    public void StartLocalGame()
    {
        if (gameSetupHandler.AllSelected)
        {
            StartGame();
        }
    }

    private void StartLocalGameWithDefaultSetup()
    {
        Timer.InitTime(defaultTimer);
        Board.selectedMapType = defaultMap;
        StartGame();
    }

    private void StartGame()
    {
        GameEvents.StartGame();
    }
}
