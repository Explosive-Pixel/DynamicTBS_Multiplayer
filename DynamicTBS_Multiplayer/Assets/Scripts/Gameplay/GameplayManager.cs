using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    // Config
    private static int maxActionsPerRound = 2;

    private GameManager gameManager;
    private PlayerManager playerManager;

    private int remainingActions;

    private void Awake()
    {
        SubscribeEvents();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
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
            playerManager.NextPlayer();
            ResetRemainingActions();
        }
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        
    }

    private void UnsubscribeEvents()
    {
       
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}