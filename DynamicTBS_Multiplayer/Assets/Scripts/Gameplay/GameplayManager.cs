using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private static int maxActionsPerRound = 2;

    private int remainingActions;

    private void Awake()
    {
        SubscribeEvents();
        ResetRemainingActions();
    }

    private void ResetRemainingActions() 
    {
        remainingActions = maxActionsPerRound;
    }

    private void OnActionFinished() 
    {
        remainingActions--;
        if (remainingActions == 0)
        {
            PlayerManager.NextPlayer();
            ResetRemainingActions();
        }
    }

    private void ListenToFinishedActions()
    {
        GameplayEvents.OnFinishAction += OnActionFinished;
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += ListenToFinishedActions;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= ListenToFinishedActions;
        GameplayEvents.OnFinishAction -= OnActionFinished;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}