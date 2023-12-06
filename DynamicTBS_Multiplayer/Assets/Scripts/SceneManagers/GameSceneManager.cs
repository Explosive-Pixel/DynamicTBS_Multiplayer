using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button backToLobbyButton;
    [SerializeField] private List<Button> backToMainMenuButtons;

    private void Awake()
    {
        playAgainButton.gameObject.SetActive(GameManager.GameType == GameType.LOCAL);
        backToLobbyButton.gameObject.SetActive(GameManager.GameType == GameType.ONLINE);
        backToMainMenuButtons.ForEach(button => button.gameObject.SetActive(!GameManager.IsPlayer()));
    }
}