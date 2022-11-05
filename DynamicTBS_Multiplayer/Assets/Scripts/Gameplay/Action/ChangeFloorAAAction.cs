using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFloorAAAction : MonoBehaviour, IAction
{
    [SerializeField]
    private GameObject changeFloorPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> changeFloorTargets = new List<GameObject>();
    public List<GameObject> ActionDestinations { get { return changeFloorTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    public void CreateActionDestinations(Character character)
    {
        Tile characterTile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);

        List<Tile> changeFlootWithInhabitantTiles = Board.GetAllTilesWithinRadius(characterTile, ChangeFloorAA.radiusWithInhabitants);
        List<Tile> changeFloorWithoutInhabitantTiles = Board.GetAllTilesWithinRadius(characterTile, ChangeFloorAA.radius)
            .FindAll(tile => !changeFlootWithInhabitantTiles.Contains(tile) && !tile.IsOccupied());

        List<Vector3> changeFloorPositions = Enumerable.Union(changeFlootWithInhabitantTiles, changeFloorWithoutInhabitantTiles)
            .ToList()
            .FindAll(tile => tile.isChangeable())
            .ConvertAll(tile => tile.GetPosition());

        if (changeFloorPositions.Count > 0)
        {
            changeFloorTargets = ActionUtils.InstantiateActionPositions(changeFloorPositions, changeFloorPrefab);
            characterInAction = character;
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            if (tile.IsOccupied())
            {
                tile.GetCurrentInhabitant().Die();
            }

            tile.Transform(OtherTileType(tile.GetTileType()));

        }

        AbortAction();
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
}
