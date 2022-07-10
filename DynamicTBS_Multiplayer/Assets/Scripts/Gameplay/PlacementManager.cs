using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    private PlayerManager playerManager;
    private const int MaxPlacementCount = 14;
    private int placementCount;
    private static List<int> placementOrder = new List<int>() { 1, 3, 5, 7, 8, 11 };

    private void Awake()
    {
        SubscribeEvents();
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        placementCount = 0;
    }

    private void SortCharacters()
    {
        // TODO
    }

    private void AdvancePlacementOrder()
    {
        placementCount += 1;
        
        if (placementOrder.Contains(placementCount))
            playerManager.NextPlayer();
        
        if (placementCount >= MaxPlacementCount)
            GameplayEvents.StartGameplayPhase();
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        PlacementEvents.OnAdvancePlacementOrder += AdvancePlacementOrder;
    }

    private void UnsubscribeEvents()
    {
        PlacementEvents.OnAdvancePlacementOrder -= AdvancePlacementOrder;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}