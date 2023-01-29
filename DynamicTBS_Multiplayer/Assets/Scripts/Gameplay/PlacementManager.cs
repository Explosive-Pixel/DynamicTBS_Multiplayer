using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using System.Linq;

public class PlacementManager : MonoBehaviour
{
    #region Placement Config

    private const int MaxPlacementCount = 14;
    private static readonly List<int> placementOrder = new List<int>() { 1, 3, 5, 7, 8, 11 };

    #endregion

    private static int placementCount;
    private static int placementOrderIndex;

    [SerializeField] private GameObject pinkPlacementTurnOverlay;
    [SerializeField] private GameObject bluePlacementTurnOverlay;
    private Vector3 oldPinkOverlayPosition = new Vector3(10, 0, 1.1f);
    private Vector3 oldBlueOverlayPosition = new Vector3(-10, 0, 1.1f);
    private Vector3 newOverlayPosition = new Vector3(0, 0, 1.1f);

    private void Awake()
    {
        SubscribeEvents();
        placementCount = 0;
        placementOrderIndex = 0;
    }

    private void SortCharacters(List<Character> characters)
    {
        Vector3 blueStartPosition = new Vector3(-7.5f, 3, 1);
        Vector3 pinkStartPosition = new Vector3(7.5f, 3, 1);
        float verticalOffset = 1;

        foreach (Character character in characters)
        {
            if (character.GetSide().GetPlayerType() == PlayerType.blue)
            {
                character.GetCharacterGameObject().transform.position = blueStartPosition;
                blueStartPosition.y -= verticalOffset;
            }
            else
            {
                character.GetCharacterGameObject().transform.position = pinkStartPosition;
                pinkStartPosition.y -= verticalOffset;
            }
        }

        PlacementEvents.StartPlacement();
        pinkPlacementTurnOverlay.transform.position = newOverlayPosition;
        bluePlacementTurnOverlay.transform.position = newOverlayPosition;
        pinkPlacementTurnOverlay.SetActive(true);
    }

    private void AdvancePlacementOrder(ActionMetadata actionMetadata)
    {
        PlacementEvents.CharacterPlaced(actionMetadata.CharacterInAction);
        actionMetadata.CharacterInAction.isClickable = false;
        placementCount += 1;
        
        if (placementOrder.Contains(placementCount))
        {
            placementOrderIndex++;
            PlayerManager.NextPlayer();
            PlacementEvents.ChangePlacementMessage();
            if (pinkPlacementTurnOverlay.activeSelf)
            {
                pinkPlacementTurnOverlay.SetActive(false);
                bluePlacementTurnOverlay.SetActive(true);
            }
            else
            {
                pinkPlacementTurnOverlay.SetActive(true);
                bluePlacementTurnOverlay.SetActive(false);
            }
        }
            
        if (placementCount >= MaxPlacementCount)
        {
            SpawnMasters();
            GameplayEvents.StartGameplayPhase();
            pinkPlacementTurnOverlay.SetActive(false);
            bluePlacementTurnOverlay.SetActive(false);
            pinkPlacementTurnOverlay.transform.position = oldPinkOverlayPosition;
            bluePlacementTurnOverlay.transform.position = oldBlueOverlayPosition;
        }    
    }

    public static void RandomPlacement(Player side)
    {
        List<Character> charactersOfPlayer = CharacterHandler.GetAllLivingCharacters()
                .FindAll(character => character.isClickable && character.GetSide() == side);
        if(charactersOfPlayer.Count > 0)
        {
            Character randomCharacter = charactersOfPlayer[0];
            ActionUtils.InstantiateAllActionPositions(randomCharacter);
            List<GameObject> placementPositions = ActionRegistry.GetActions().ConvertAll(action => action.ActionDestinations).SelectMany(i => i).ToList();
            GameObject randomPosition = placementPositions[RandomNumberGenerator.GetInt32(0, placementPositions.Count)];
            ActionUtils.ExecuteAction(randomPosition);
        } 
    }
    
    public static void SpawnMasters()
    {
        SpawnMaster(PlayerType.blue);
        SpawnMaster(PlayerType.pink);
    }

    public static int GetCurrentPlacementCount()
    {
        if (placementOrderIndex == 0)
        {
            return placementOrder[placementOrderIndex];
        }
        else if (placementOrderIndex == placementOrder.Count)
        {
            return MaxPlacementCount - placementOrder[placementOrderIndex - 1];
        }
        return placementOrder[placementOrderIndex] - placementOrder[placementOrderIndex - 1];
    }

    private static void SpawnMaster(PlayerType playerType) 
    {
        Character master = CharacterFactory.CreateCharacter(CharacterType.MasterChar, PlayerManager.GetPlayer(playerType));

        Tile masterSpawnTile = Board.FindMasterStartTile(playerType);
        Vector3 position = masterSpawnTile.GetPosition();
        master.GetCharacterGameObject().transform.position = new Vector3(position.x, position.y, 0.998f);
        masterSpawnTile.SetCurrentInhabitant(master);
        DraftEvents.CharacterCreated(master);
        PlacementEvents.CharacterPlaced(master);
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnFinishAction += AdvancePlacementOrder;
        DraftEvents.OnDeliverCharacterList += SortCharacters;
        GameplayEvents.OnGameplayPhaseStart += UnsubscribeEvents;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnFinishAction -= AdvancePlacementOrder;
        DraftEvents.OnDeliverCharacterList -= SortCharacters;
        GameplayEvents.OnGameplayPhaseStart -= UnsubscribeEvents;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}