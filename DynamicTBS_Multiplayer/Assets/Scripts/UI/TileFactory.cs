using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFactory : MonoBehaviour
{
    public static Tile CreateTile(TileType type, Vector3 position)
    {
        Tile tile = null;
        switch (type)
        {
            case TileType.EmptyTile:
                tile = new EmptyTile(position);
                break;
            case TileType.FloorTile:
                tile = new FloorTile(position);
                break;
            case TileType.GoalTile:
                tile = new GoalTile(position);
                break;
            case TileType.StartTile:
                tile = new StartTile(position);
                break;
            case TileType.MasterStartTile:
                tile = new MasterStartTile(position);
                break;
        }

        if (tile != null) 
        {
            Instantiate(tile.GetTileGameObject());
            return tile;
        }

        return null;
    }
}