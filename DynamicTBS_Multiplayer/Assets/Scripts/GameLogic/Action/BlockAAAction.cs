using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject blockActionPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> blockTargets = new();
    public List<GameObject> ActionDestinations { get { return blockTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    public void ShowActionPattern(Character character)
    {
        Tile characterTile = Board.GetTileByCharacter(character);
        List<Vector3> patternPositions = Board.GetTilesOfDistance(characterTile, BlockAA.pattern, BlockAA.distance).ConvertAll(tile => tile.gameObject.transform.position);

        if (patternPositions != null)
        {
            patternTargets = ActionUtils.InstantiateActionPositions(this, patternPositions, blockActionPrefab);
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
            blockTargets = ActionUtils.InstantiateActionPositions(this, floorPositions, blockActionPrefab);
            characterInAction = character;
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            tile.Transform(TileType.EmptyTile);
            ((BlockAA)characterInAction.ActiveAbility).ActivateBlock();
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
        Tile characterTile = Board.GetTileByCharacter(character);

        List<Tile> floorTiles = Board.GetTilesOfDistance(characterTile, BlockAA.pattern, BlockAA.distance)
            .FindAll(tile => tile.IsNormalFloor() && !tile.IsOccupied());

        List<Vector3> floorPositions = floorTiles.ConvertAll(tile => tile.gameObject.transform.position);

        return floorPositions;
    }
}
