using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;

public class WinScreenHandler : MonoBehaviour
{
    [System.Serializable]
    public class ConditionEntry
    {
        public GameOverCondition condition;
        public PlayerType winner;
        public LocalizedString localizedString;
    }

    [Header("UI Objects")]
    [SerializeField] private GameObject blueWin;
    [SerializeField] private GameObject pinkWin;
    [SerializeField] private GameObject draw;
    [SerializeField] private GameObject rematchButton_online;
    [SerializeField] private GameObject rematchButton_offline;
    [SerializeField] private GameObject toLobbyButton;

    [Header("Text Output")]
    [SerializeField] private TMP_Text gameOverText;

    [Header("Localized Strings")]
    [SerializeField] private ConditionEntry[] conditionEntries;

    private PlayerType? lastWinner = null;
    private GameOverCondition lastCondition;

    private void Awake()
    {
        GameplayEvents.OnGameOver += InitWinScreen;
        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        GameplayEvents.OnGameOver -= InitWinScreen;
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    }

    private void InitWinScreen(PlayerType? winner, GameOverCondition endGameCondition)
    {
        lastWinner = winner;
        lastCondition = endGameCondition;

        rematchButton_offline.SetActive(GameManager.GameType == GameType.LOCAL);
        rematchButton_online.SetActive(GameManager.GameType == GameType.ONLINE && GameManager.IsPlayer());
        toLobbyButton.SetActive(GameManager.GameType == GameType.ONLINE && GameManager.IsSpectator());

        UpdateLocalizedText();
    }

    public void ClickOnRematchButton()
    {
        MenuEvents.ClickOnRematch();
    }

    /// <summary>
    /// Wird aufgerufen, wenn die Sprache geändert wird.
    /// </summary>
    private void OnLanguageChanged(Locale _)
    {
        if (lastWinner != null)
            UpdateLocalizedText();
    }

    /// <summary>
    /// Setzt UI + Text basierend auf Winner + Condition.
    /// Wird bei GameOver UND Sprachwechsel aufgerufen.
    /// </summary>
    private void UpdateLocalizedText()
    {
        gameOverText.text = "";

        // Update UI visibility
        blueWin.SetActive(lastWinner == PlayerType.blue);
        pinkWin.SetActive(lastWinner == PlayerType.pink);
        draw.SetActive(lastWinner == null);

        // No condition text in case of draw
        if (lastWinner == null)
            return;

        // Find matching entry
        foreach (var entry in conditionEntries)
        {
            if (entry.condition == lastCondition &&
                entry.winner == lastWinner)
            {
                var handle = entry.localizedString.GetLocalizedStringAsync();
                handle.Completed += op =>
                {
                    gameOverText.text = op.Result;
                };
                return;
            }
        }

        Debug.LogWarning($"[WinScreenHandler] Kein LocalizedString für {lastCondition} / Winner {lastWinner} gefunden.");
    }
}
