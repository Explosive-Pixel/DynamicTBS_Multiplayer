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
    [SerializeField] private float characterScaling;

    private static readonly List<int> PlacementSequence = new();
    private static int placementSequenceIndex;
    private static int currentPlayerPlacementCount;
    private static Vector3 characterScaleVector;

    public static int CurrentPlayerTotalPlacementCount { get { return placementSequenceIndex < PlacementSequence.Count ? PlacementSequence[placementSequenceIndex] : 0; } }
    public static int CurrentPlayerRemainingPlacementCount { get { return CurrentPlayerTotalPlacementCount - currentPlayerPlacementCount; } }
    public static int MaxPlacementCount { get { return PlacementSequence.Sum(); } }
    private List<Vector3> CharacterPositions(PlayerType side) { return side == PlayerType.blue ? blueCharacterPositions : pinkCharacterPositions; }

    private static bool init = false;

    private void Awake()
    {
        if (!init)
        {
            PlacementSequence.AddRange(placementSequence);
            characterScaleVector = new Vector3(characterScaling, characterScaling, 1);

            init = true;
        }

        placementSequenceIndex = 0;
        currentPlayerPlacementCount = 0;

        SubscribeEvents();
    }

    private void AdvancePlacementOrder(ActionMetadata actionMetadata)
    {
        PlacementEvents.CharacterPlaced(actionMetadata.CharacterInAction);
        actionMetadata.CharacterInAction.IsClickable = false;

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

    public static void RandomPlacements(PlayerType side)
    {
        int i = GetRemainingPlacementCount(side);
        while (i-- > 0)
        {
            RandomPlacement(side);
        }
    }

    public static void RandomPlacement(PlayerType side)
    {
        List<Character> charactersOfPlayer = CharacterManager.GetAllLivingCharactersOfSide(side)
                .FindAll(character => character.IsClickable);
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

    public static int GetRemainingPlacementCount(PlayerType currentPlayer)
    {
        if (PlayerManager.CurrentPlayer != currentPlayer)
        {
            return 0;
        }

        return CurrentPlayerTotalPlacementCount - currentPlayerPlacementCount;
    }

    private void SpawnMaster(PlayerType playerType)
    {
        Character master = CharacterFactory.CreateCharacter(CharacterType.CaptainChar, playerType);
        master.gameObject.transform.localScale = characterScaleVector;
        Tile masterSpawnTile = Board.Tiles.Find(tile => tile.TileType == TileType.MasterStartTile && tile.Side == playerType);

        MoveAction.MoveCharacter(master, masterSpawnTile);

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
        if (gamePhase != GamePhase.PLACEMENT)
            return;

        SortCharactersOfSide(PlayerType.blue);
        SortCharactersOfSide(PlayerType.pink);
    }

    private void SortCharactersOfSide(PlayerType side)
    {
        List<Character> characters = CharacterManager.GetAllLivingCharactersOfSide(side);
        List<Vector3> positions = CharacterPositions(side);

        for (int i = 0; i < characters.Count(); i++)
        {
            characters[i].gameObject.transform.position = positions[i];
            characters[i].gameObject.transform.localScale = characterScaleVector;
        }

        Camera.main.orthographicSize = 1.63f;
        GameObject.Find("Background").transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
    }

    #endregion

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnFinishAction += AdvancePlacementOrder;
        GameEvents.OnGamePhaseStart += SortCharacters;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnFinishAction -= AdvancePlacementOrder;
        GameEvents.OnGamePhaseStart -= SortCharacters;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}