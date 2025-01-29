using UnityEngine;
using UnityEngine.UI;

public class WinScreenHandler : MonoBehaviour
{
    [SerializeField] private GameObject blueWin;
    [SerializeField] private GameObject pinkWin;
    [SerializeField] private GameObject draw;

    [SerializeField] private Text gameOverText;

    private void Awake()
    {
        GameplayEvents.OnGameOver += InitWinScreen;
    }

    private void InitWinScreen(PlayerType? winner, GameOverCondition endGameCondition)
    {
        gameOverText.gameObject.SetActive(false);

        if (winner != null)
        {
            gameOverText.text = endGameCondition.ToText(winner);
            gameOverText.gameObject.SetActive(true);
        }

        blueWin.SetActive(winner == PlayerType.blue);
        pinkWin.SetActive(winner == PlayerType.pink);
        draw.SetActive(winner == null);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnGameOver -= InitWinScreen;
    }
}
