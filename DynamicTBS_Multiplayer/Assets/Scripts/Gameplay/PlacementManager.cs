using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using System.Linq;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private List<Vector3> blueCharacterPositions;
    [SerializeField] private List<Vector3> pinkCharacterPositions;
    [SerializeField] private List<int> placementSequence;

    private static readonly List<int> PlacementSequence = new();
    private static int placementSequenceIndex;
    private static int currentPlayerPlacementCount;

    public static int CurrentPlayerTotalPlacementCount { get { return placementSequenceIndex < PlacementSequence.Count ? PlacementSequence[placementSequenceIndex] : 0; } }
    public static int MaxPlacementCount { get { return PlacementSequence.Sum(); } }
    private List<Vector3> CharacterPositions(PlayerType side) { return side == PlayerType.blue ? blueCharacterPositions : pinkCharacterPositions; }

    private bool init = false;

    private void Awake()
    {
        if (!init)
        {
            PlacementSequence.AddRange(placementSequence);

            init = true;
        }

        placementSequenceIndex = 0;
        currentPlayerPlacementCount = 0;

        SubscribeEvents();
    }

    private void AdvancePlacementOrder(ActionMetadata actionMetadata)
    {
        PlacementEvents.CharacterPlaced(actionMetadata.CharacterInAction);
        actionMetadata.CharacterInAction.isClickable = false;

        currentPlayerPlacementCount++;

        if (currentPlayerPlacementCount == CurrentPlayerTotalPlacementCount)
        {
            currentPlayerPlacementCount = 0;
            placementSequenceIndex++;
            PlayerManager.NextPlayer();
        }

        if (placementSequenceIndex == PlacementSequence.Count)
        {
            SpawnMasters();
            PlacementCompleted();
        }
    }

    public static void RandomPlacements(Player side)
    {
        int i = GetRemainingPlacementCount(side);
        while (i-- > 0)
        {
            RandomPlacement(side);
        }
    }

    public static void RandomPlacement(Player side)
    {
        List<Character> charactersOfPlayer = CharacterHandler.GetAllLivingCharacters()
                .FindAll(character => character.isClickable && character.GetSide() == side);
        if (charactersOfPlayer.Count > 0)
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

    public static int GetRemainingPlacementCount(Player currentPlayer)
    {
        if (PlayerManager.GetCurrentPlayer() != currentPlayer)
        {
            return 0;
        }

        return CurrentPlayerTotalPlacementCount - currentPlayerPlacementCount;
    }

    private void SpawnMaster(PlayerType playerType)
    {
        CharacterMB master = CharacterFactoryMB.CreateCharacter(CharacterType.MasterChar, playerType);
        TileMB masterSpawnTile = BoardNew.Tiles.Find(tile => tile.TileType == TileType.MasterStartTile && tile.Side == playerType);

        MoveAction.MoveCharacter(master, masterSpawnTile);

        DraftEvents.CharacterCreated(master);
        //PlacementEvents.CharacterPlaced(master);
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

        SortCharactersOfSide(PlayerType.blue);
        SortCharactersOfSide(PlayerType.pink);

        GameManager.ChangeGamePhase(GamePhase.PLACEMENT);
    }

    private void SortCharactersOfSide(PlayerType side)
    {
        List<CharacterMB> characters = CharacterManager.GetAllLivingCharactersOfSide(side);
        List<Vector3> positions = CharacterPositions(side);

        for (int i = 0; i < characters.Count(); i++)
        {
            characters[i].gameObject.transform.position = positions[i];
        }
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