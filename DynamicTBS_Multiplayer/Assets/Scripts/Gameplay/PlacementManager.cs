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
        Debug.Log(placementCount);
    }

    private void SortCharacters()
    {
        // TODO Characters will be sorted to sides after Draft is over.
    }

    private void AdvancePlacementOrder()
    {
        placementCount += 1;
        Debug.Log(placementCount);
        
        if (placementOrder.Contains(placementCount))
            playerManager.NextPlayer();

        if (placementCount >= MaxPlacementCount)
        {
            SetUpMasters();
            GameplayEvents.StartGameplayPhase();
        }
            
    }
    
    private void SetUpMasters()
    {
        Player playerBlue = playerManager.GetPlayer(PlayerType.blue);
        Character blueMaster = CharacterFactory.CreateCharacter(CharacterType.MasterChar, playerBlue);

        Player playerPink = playerManager.GetPlayer(PlayerType.pink);
        Character pinkMaster = CharacterFactory.CreateCharacter(CharacterType.MasterChar, playerPink);

        Tile blueMasterSpawnTile = board.FindMasterStartTile(PlayerType.blue);
        Tile pinkMasterSpawnTile = board.FindMasterStartTile(PlayerType.pink);

        GameObject blueMasterObject = blueMaster.GetCharacterGameObject();
        GameObject pinkMasterObject = pinkMaster.GetCharacterGameObject();
        
        blueMasterObject.transform.position = blueMasterSpawnTile.GetTileGameObject().transform.position;
        blueMasterObject.transform.position = new Vector3(blueMasterObject.transform.position.x, blueMasterObject.transform.position.y, 0.997f);
        blueMasterSpawnTile.SetCurrentInhabitant(blueMaster);
        
        pinkMasterObject.transform.position = pinkMasterSpawnTile.GetTileGameObject().transform.position;
        pinkMasterObject.transform.position = new Vector3(pinkMasterObject.transform.position.x, pinkMasterObject.transform.position.y, 0.997f);
        pinkMasterSpawnTile.SetCurrentInhabitant(pinkMaster);
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