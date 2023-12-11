using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionInfoHandler : MonoBehaviour
{
    [SerializeField] private GamePhase gamePhase;

    private void Awake()
    {
        Reset();
        GameEvents.OnGamePhaseStart += SetActive;
        GameEvents.OnGamePhaseEnd += SetInactive;
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

    private void UpdateDraftCounter()
    {
        UpdateCounter("Units to draft: " + DraftManager.CurrentPlayerRemainingDraftCount);
    }

    private void UpdatePlacementCounter()
    {
        UpdateCounter("Units to place: " + PlacementManager.CurrentPlayerRemainingPlacementCount);
    }

    private void UpdateGameplayCounter()
    {
        UpdateCounter("Actions: " + GameplayManager.GetRemainingActions().ToString());
    }

    private void UpdateCounter(string text)
    {
        var textField = gameObject.GetComponentInChildren<TMPro.TextMeshPro>();

        if (textField == null)
            return;

        textField.text = text;
    }

    private void Reset()
    {
        gameObject.GetComponentsInChildren<TMPro.TextMeshPro>(true).ToList().ForEach(go => go.text = "");
    }

    private void SetInactive(GamePhase lastGamePhase)
    {
        DraftEvents.OnDraftActionFinished -= UpdateDraftCounter;
        GameplayEvents.OnFinishAction -= UpdatePlacementCounter;
        GameplayEvents.OnChangeRemainingActions -= UpdateGameplayCounter;
    }

    private void UpdatePlacementCounter(ActionMetadata actionMetadata)
    {
        UpdatePlacementCounter();
    }

    private void OnDestroy()
    {
        SetInactive(gamePhase);

        GameEvents.OnGamePhaseStart -= SetActive;
        GameEvents.OnGamePhaseEnd -= SetInactive;
    }
}
