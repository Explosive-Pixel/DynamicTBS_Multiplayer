using System.Collections.Generic;
using UnityEngine;

public class ChangeFloorAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject changeFloorPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> changeFloorTargets = new();
    public List<GameObject> ActionDestinations { get { return changeFloorTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    public void ShowActionPattern(Character character)
    {
        Tile characterTile = Board.GetTileByCharacter(character);
        List<Vector3> patternPositions = Board.GetTilesInAllDirections(characterTile, ChangeFloorAA.pattern, ChangeFloorAA.range).ConvertAll(tile => tile.gameObject.transform.position);

        if (patternPositions != null)
        {
            patternTargets = ActionUtils.InstantiateActionPositions(this, patternPositions, changeFloorPrefab);
        }
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(Character character)
    {
        List<Vector3> changeFloorPositions = FindChangeFloorPositions(character);

        if (changeFloorPositions != null)
        {
            return changeFloorPositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(Character character)
    {
        List<Vector3> changeFloorPositions = FindChangeFloorPositions(character);

        if (changeFloorPositions != null && changeFloorPositions.Count > 0)
        {
            changeFloorTargets = ActionUtils.InstantiateActionPositions(this, changeFloorPositions, changeFloorPrefab);
            characterInAction = character;
        }
    }

    public bool ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            tile.Transform(OtherTileType(tile.TileType));

            if (tile.IsOccupied() && tile.IsHole())
            {
                tile.CurrentInhabitant.Die();
            }
        }

        AbortAction();

        return true;
    }

    public void AbortAction()
    {
        ActionUtils.Clear(changeFloorTargets);
        ActionRegistry.Remove(this);
        characterInAction = null;
    }

    private TileType OtherTileType(TileType tileType)
    {
        return tileType == TileType.EmptyTile ? TileType.FloorTile : TileType.EmptyTile;
    }

    private List<Vector3> FindChangeFloorPositions(Character character)
    {
        Tile characterTile = Board.GetTileByCharacter(character);

        List<Tile> changeFloorTilesWithInhabitant = Board.GetTilesOfClosestCharactersOfSideInAllDirections(characterTile, PlayerType.none, ChangeFloorAA.pattern, ChangeFloorAA.rangeWithInhabitants);
        List<Tile> changeFloorTiles = Board.GetTilesUntilClosestCharactersInAllDirections(characterTile, ChangeFloorAA.pattern, ChangeFloorAA.range)
            .FindAll(tile => changeFloorTilesWithInhabitant.Contains(tile) || !tile.IsOccupied());

        List<Vector3> changeFloorPositions = changeFloorTiles
            .FindAll(tile => tile.isChangeable())
            .ConvertAll(tile => tile.gameObject.transform.position);

        return changeFloorPositions;
    }
}
