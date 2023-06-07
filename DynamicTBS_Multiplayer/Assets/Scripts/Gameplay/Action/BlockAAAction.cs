using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAAAction : MonoBehaviour, IAction
{
    [SerializeField]
    private GameObject blockActionPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility;} }

    private List<GameObject> blockTargets = new List<GameObject>();
    public List<GameObject> ActionDestinations { get { return blockTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new List<GameObject>();

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += Register;
    }

    public void ShowActionPattern(Character character)
    {
        Tile characterTile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);
        List<Vector3> patternPositions = Board.GetTilesOfDistance(characterTile, BlockAA.pattern, BlockAA.distance).ConvertAll(tile => tile.GetPosition());

        if (patternPositions != null)
        {
            patternTargets = ActionUtils.InstantiateActionPositions(patternPositions, blockActionPrefab);
        }
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(Character character)
    {
        List<Vector3> floorPositions = FindFloorPositions(character);

        if (floorPositions != null)
        {
            return floorPositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(Character character)
    {
        List<Vector3> floorPositions = FindFloorPositions(character);

        if (floorPositions != null && floorPositions.Count > 0)
        {
            blockTargets = ActionUtils.InstantiateActionPositions(floorPositions, blockActionPrefab);
            characterInAction = character;
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if(tile != null)
        {
            tile.Transform(TileType.EmptyTile);
            ((BlockAA)characterInAction.GetActiveAbility()).ActivateBlock();
        }

        AbortAction();
    }

    public void AbortAction()
    {
        ActionUtils.Clear(blockTargets);
        ActionRegistry.Remove(this);
        characterInAction = null;
    }

    private List<Vector3> FindFloorPositions(Character character)
    {
        Tile characterTile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);

        List<Tile> floorTiles = Board.GetTilesOfDistance(characterTile, BlockAA.pattern, BlockAA.distance)
            .FindAll(tile => tile.IsNormalFloor() && !tile.IsOccupied());

        List<Vector3> floorPositions = floorTiles.ConvertAll(tile => tile.GetPosition());

        return floorPositions;
    }

    private void Register(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.GAMEPLAY)
            return;

        ActionRegistry.RegisterPatternAction(this);
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= Register;
    }
}
