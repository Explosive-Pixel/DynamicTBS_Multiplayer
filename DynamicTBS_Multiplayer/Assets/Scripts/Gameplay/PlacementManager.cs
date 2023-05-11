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

    private List<Vector3> blueSortingPositionsList = new List<Vector3>() { 
    new Vector3(-7.5f, -0.5f, 1),
    new Vector3(-7.5f, -1.5f, 1),
    new Vector3(-7.5f, -2.5f, 1),
    new Vector3(-7.5f, -3.5f, 1),
    new Vector3(-6.5f, -0.5f, 1),
    new Vector3(-6.5f, -1.5f, 1),
    new Vector3(-6.5f, -2.5f, 1)};

    private List<Vector3> pinkSortingPositionsList = new List<Vector3>() {
    new Vector3(7.5f, 3.5f, 1),
    new Vector3(7.5f, 2.5f, 1),
    new Vector3(7.5f, 1.5f, 1),
    new Vector3(7.5f, 0.5f, 1),
    new Vector3(6.5f, 3.5f, 1),
    new Vector3(6.5f, 2.5f, 1),
    new Vector3(6.5f, 1.5f, 1)};

    #endregion

    private static int placementCount;
    private static int placementOrderIndex;

    [SerializeField]
    private Board board;

    private void Awake()
    {
        SubscribeEvents();
        placementCount = 0;
        placementOrderIndex = 0;
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
        }
            
        if (placementCount >= MaxPlacementCount)
        {
            SpawnMasters();
            PlacementCompleted();
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
    
    public void SpawnMasters()
    {
        SpawnMaster(PlayerType.blue);
        SpawnMaster(PlayerType.pink);
        AudioEvents.SpawningMasters();
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

    private void SpawnMaster(PlayerType playerType) 
    {
        Character master = CharacterFactory.CreateCharacter(CharacterType.MasterChar, PlayerManager.GetPlayer(playerType));

        Tile masterSpawnTile = board.FindMasterStartTile(playerType);
        Vector3 position = masterSpawnTile.GetPosition();
        master.GetCharacterGameObject().transform.position = new Vector3(position.x, position.y, 1f);
        masterSpawnTile.SetCurrentInhabitant(master);
        DraftEvents.CharacterCreated(master);
        PlacementEvents.CharacterPlaced(master);
    }

    private void PlacementCompleted()
    {
        GameEvents.EndGamePhase(GamePhase.PLACEMENT);
        UnsubscribeEvents();
    }

    #region UI
    private void SortCharacters(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.DRAFT)
            return;

        List<Character> blueCharacters = new List<Character>();
        List<Character> pinkCharacters = new List<Character>();

        foreach (Character character in CharacterHandler.Characters)
        {
            if (character.GetSide().GetPlayerType() == PlayerType.blue)
            {
                blueCharacters.Add(character);
            }
            else
            {
                pinkCharacters.Add(character);
            }
        }

        for (int i = 0; i < blueCharacters.Count(); i++)
        {
            blueCharacters[i].GetCharacterGameObject().transform.position = blueSortingPositionsList[i];
        }

        for (int i = 0; i < pinkCharacters.Count(); i++)
        {
            pinkCharacters[i].GetCharacterGameObject().transform.position = pinkSortingPositionsList[i];
        }

        GameManager.ChangeGamePhase(GamePhase.PLACEMENT);
    }
    #endregion

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnFinishAction += AdvancePlacementOrder;
        GameEvents.OnGamePhaseEnd += SortCharacters;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnFinishAction -= AdvancePlacementOrder;
        GameEvents.OnGamePhaseEnd -= SortCharacters;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}