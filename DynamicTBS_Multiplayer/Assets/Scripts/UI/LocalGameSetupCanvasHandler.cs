using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LocalGameSetupCanvasHandler : MonoBehaviour
{
    [SerializeField] private Button startGameButton;

    [SerializeField] private GameSetupHandler gameSetupHandler;

    private bool active = false;

    private void Awake()
    {
        if (GameManager.gameType == GameType.LOCAL)
        {
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
            GameEvents.StartGame();
        }
    }
}
