using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ActionInfoHandler : MonoBehaviour
{
    [SerializeField] private GamePhase gamePhase;

    [Header("Localized Strings")]
    [SerializeField] private LocalizedString draftCounterString;
    [SerializeField] private LocalizedString placementCounterString;
    [SerializeField] private LocalizedString gameplayCounterString;

    // Letzte Werte merken, damit wir sie bei Sprachwechsel erneut setzen können
    private int lastDraftCount;
    private int lastPlacementCount;
    private int lastGameplayCount;

    private void Awake()
    {
        ResetText();
        GameEvents.OnGamePhaseStart += SetActive;
        GameEvents.OnGamePhaseEnd += SetInactive;

        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDestroy()
    {
        SetInactive(gamePhase);
        GameEvents.OnGamePhaseStart -= SetActive;
        GameEvents.OnGamePhaseEnd -= SetInactive;

        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale _)
    {
        // Sprachwechsel → Text erneut setzen
        switch (gamePhase)
        {
            case GamePhase.DRAFT:
                UpdateCounterText(draftCounterString, new { count = lastDraftCount });
                break;
            case GamePhase.PLACEMENT:
                UpdateCounterText(placementCounterString, new { count = lastPlacementCount });
                break;
            case GamePhase.GAMEPLAY:
                UpdateCounterText(gameplayCounterString, new { count = lastGameplayCount });
                break;
        }
    }

    private void SetActive(GamePhase currentGamePhase)
    {
        if (gamePhase != currentGamePhase)
            return;

        switch (currentGamePhase)
        {
            case GamePhase.DRAFT:
                DraftEvents.OnDraftActionFinished += UpdateDraftCounter;
                UpdateDraftCounter();
                break;
            case GamePhase.PLACEMENT:
                GameplayEvents.OnFinishAction += UpdatePlacementCounter;
                UpdatePlacementCounter();
                break;
            case GamePhase.GAMEPLAY:
                GameplayEvents.OnChangeRemainingActions += UpdateGameplayCounter;
                UpdateGameplayCounter();
                break;
        }
    }

    private void SetInactive(GamePhase lastGamePhase)
    {
        DraftEvents.OnDraftActionFinished -= UpdateDraftCounter;
        GameplayEvents.OnFinishAction -= UpdatePlacementCounter;
        GameplayEvents.OnChangeRemainingActions -= UpdateGameplayCounter;
    }

    private void UpdateDraftCounter()
    {
        lastDraftCount = DraftManager.CurrentPlayerRemainingDraftCount;
        UpdateCounterText(draftCounterString, new { count = lastDraftCount });
    }

    private void UpdatePlacementCounter()
    {
        lastPlacementCount = PlacementManager.CurrentPlayerRemainingPlacementCount;
        UpdateCounterText(placementCounterString, new { count = lastPlacementCount });
    }

    private void UpdatePlacementCounter(Action action)
    {
        UpdatePlacementCounter();
    }

    private void UpdateGameplayCounter()
    {
        lastGameplayCount = GameplayManager.GetRemainingActions();
        UpdateCounterText(gameplayCounterString, new { count = lastGameplayCount });
    }

    private void UpdateCounterText(LocalizedString localizedString, object arguments)
    {
        var textField = gameObject.GetComponentInChildren<TMPro.TextMeshPro>();
        if (textField == null || localizedString == null)
            return;

        var handle = localizedString.GetLocalizedStringAsync(arguments);
        handle.Completed += op =>
        {
            textField.text = op.Result;
        };
    }

    private void ResetText()
    {
        gameObject.GetComponentsInChildren<TMPro.TextMeshPro>(true)
            .ToList()
            .ForEach(go => go.text = "");
    }
}
