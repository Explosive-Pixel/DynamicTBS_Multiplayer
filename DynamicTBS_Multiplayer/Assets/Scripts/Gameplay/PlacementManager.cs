using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    private const int MaxPlacementCount = 14;
    private static List<int> placementOrder = new List<int>() { 1, 3, 5, 7, 8, 11 };
    
    private PlayerManager playerManager;
    private Board board;
    
    private int placementCount;

    private void Awake()
    {
        SubscribeEvents();
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        board = GameObject.Find("GameplayCanvas").GetComponent<Board>();
        placementCount = 0;
    }

    private void SortCharacters()
    {
        // TODO Characters will be sorted to sides after Draft is over.
    }

    private void AdvancePlacementOrder()
    {
        placementCount += 1;
        
        if (placementOrder.Contains(placementCount))
            playerManager.NextPlayer();

        if (placementCount >= MaxPlacementCount)
        {
            SpawnMasters();
            GameplayEvents.StartGameplayPhase();
        }
            
    }
    
    private void SpawnMasters()
    {
        SpawnMaster(PlayerType.blue);
        SpawnMaster(PlayerType.pink);
    }

    private void SpawnMaster(PlayerType playerType) 
    {
        Character master = CharacterFactory.CreateCharacter(CharacterType.MasterChar, playerManager.GetPlayer(playerType));

        Tile masterSpawnTile = board.FindMasterStartTile(playerType);
        Vector3 position = masterSpawnTile.GetPosition();
        master.GetCharacterGameObject().transform.position = new Vector3(position.x, position.y, 0.997f);
        masterSpawnTile.SetCurrentInhabitant(master);
        DraftEvents.CharacterCreated(master);
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