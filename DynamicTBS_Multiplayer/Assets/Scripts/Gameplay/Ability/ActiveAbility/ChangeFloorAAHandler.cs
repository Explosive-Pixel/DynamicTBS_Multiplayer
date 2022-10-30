using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFloorAAHandler : MonoBehaviour
{
    private Board board;

    private bool waitForFloorToChange;

    private void Awake()
    {
        SubscribeEvents();
        waitForFloorToChange = false;
    }

    public void ExecuteChangeFloorAA(Character character)
    {
        if (board == null)
            board = GameObject.Find("GameplayCanvas").GetComponent<Board>();

        Tile characterTile = board.GetTileByPosition(character.GetCharacterGameObject().transform.position);

        List<Tile> changeFlootWithInhabitantTiles = board.GetAllTilesWithinRadius(characterTile, ChangeFloorAA.radiusWithInhabitants);
        List<Tile> changeFloorWithoutInhabitantTiles = board.GetAllTilesWithinRadius(characterTile, ChangeFloorAA.radius)
            .FindAll(tile => !changeFlootWithInhabitantTiles.Contains(tile) && !tile.IsOccupied());

        List<Vector3> changeFloorPositions = Enumerable.Union(changeFlootWithInhabitantTiles, changeFloorWithoutInhabitantTiles)
            .ToList()
            .FindAll(tile => tile.GetTileType() != TileType.MasterStartTile)
            .ConvertAll(tile => tile.GetPosition());

        UIEvents.PassActionPositionsList(changeFloorPositions, UIActionType.ActiveAbility_ChangeFloor);
        waitForFloorToChange = true;
    }

    private void ChangeFloor(Vector3 position, UIActionType type)
    {
        if (waitForFloorToChange && type == UIActionType.ActiveAbility_ChangeFloor)
        {
            Tile tile = board.GetTileByPosition(position);
            if (tile != null)
            {
                if (tile.IsOccupied())
                {
                    tile.GetCurrentInhabitant().Die();
                }

                tile.Transform(OtherTileType(tile.GetTileType()));
                
            }
            waitForFloorToChange = false;
            GameplayEvents.ActionFinished(UIActionType.ActiveAbility_ChangeFloor);
        }
    }

    private TileType OtherTileType(TileType tileType)
    {
        return tileType == TileType.EmptyTile ? TileType.FloorTile : TileType.EmptyTile;
    }


    #region EventsRegion

    private void SubscribeEvents()
    {
        UIEvents.OnPassActionDestination += ChangeFloor;
    }

    private void UnsubscribeEvents()
    {
        UIEvents.OnPassActionDestination -= ChangeFloor;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
