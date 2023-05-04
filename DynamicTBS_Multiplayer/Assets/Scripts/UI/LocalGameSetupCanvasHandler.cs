using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LocalGameSetupCanvasHandler : MonoBehaviour
{
    [SerializeField] private Button startGameButton;

    [SerializeField] private GameSetupHandler gameSetupHandler;

    private void Update()
    {
        startGameButton.interactable = gameSetupHandler.AllSelected;
    }
    public void StartLocalGame()
    {
        if(gameSetupHandler.AllSelected)
            GameEvents.StartGame();
    }
}
