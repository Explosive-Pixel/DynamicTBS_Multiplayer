using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class WinScreenHandler : MonoBehaviour
{
    [SerializeField] private GameObject blueWin;
    [SerializeField] private GameObject pinkWin;
    [SerializeField] private GameObject draw;

    [SerializeField] private Text gameOverText;
    [SerializeField] private LocalizeStringEvent winningCondition;

    private void Awake()
    {
        GameplayEvents.OnGameOver += InitWinScreen;
    }

    private void InitWinScreen(PlayerType? winner, GameOverCondition endGameCondition)
    {
        winningCondition.gameObject.SetActive(false);

        if (winner != null)
        {
            winningCondition.StringReference.TableEntryReference = endGameCondition.ToText(winner);
            winningCondition.gameObject.SetActive(true);
            winningCondition.RefreshString();
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
