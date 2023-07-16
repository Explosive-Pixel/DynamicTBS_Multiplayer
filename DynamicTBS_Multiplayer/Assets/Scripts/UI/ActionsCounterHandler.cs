using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsCounterHandler : MonoBehaviour
{
    [SerializeField] private GameObject blueActionsCounter;
    [SerializeField] private GameObject pinkActionsCounter;

    private void Awake()
    {
        SubscribeEvents();
        UpdatePlacementCounter();
    }

    private void UpdatePlacementCounter()
    {
        UpdateCounter("Units to place: " + PlacementManager.CurrentPlayerRemainingPlacementCount);
    }

    public void UpdateActionsCounter()
    {
        UpdateCounter("Actions: " + GameplayManager.GetRemainingActions().ToString());
    }

    private void UpdateCounter(string text)
    {
        CurrentActionsCounter().GetComponent<TMPro.TextMeshPro>().text = text;

        blueActionsCounter.SetActive(PlayerManager.CurrentPlayer == PlayerType.blue);
        pinkActionsCounter.SetActive(PlayerManager.CurrentPlayer == PlayerType.pink);
    }

    private GameObject CurrentActionsCounter()
    {
        if (PlayerManager.CurrentPlayer == PlayerType.blue)
        {
            return blueActionsCounter;
        }
        return pinkActionsCounter;
    }

    private void TransformToActionCounter(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.GAMEPLAY)
        {
            UpdateActionsCounter();

            GameplayEvents.OnFinishAction -= UpdatePlacementCounter;
            GameplayEvents.OnChangeRemainingActions += UpdateActionsCounter;
        }
    }

    private void UpdatePlacementCounter(ActionMetadata actionMetadata)
    {
        if (GameManager.gamePhase == GamePhase.PLACEMENT)
        {
            UpdatePlacementCounter();
        }
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnFinishAction += UpdatePlacementCounter;
        GameEvents.OnGamePhaseStart += TransformToActionCounter;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= TransformToActionCounter;
        GameplayEvents.OnChangeRemainingActions -= UpdateActionsCounter;
        GameplayEvents.OnFinishAction -= UpdatePlacementCounter;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
