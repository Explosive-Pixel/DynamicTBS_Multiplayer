using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFactory : MonoBehaviour
{
    public static Tile CreateTile(TileType type, int row, int column)
    {
        Tile tile = null;
        switch (type)
        {
            case TileType.EmptyTile:
                tile = new EmptyTile(row, column);
                break;
            case TileType.FloorTile:
                tile = new FloorTile(row, column);
                break;
            case TileType.GoalTile:
                tile = new GoalTile(row, column);
                break;
            case TileType.StartTile:
                tile = new StartTile(row, column);
                break;
            case TileType.MasterStartTile:
                tile = new MasterStartTile(row, column);
                break;
        }

        if (tile != null) 
        {
            return tile;
        }

        return null;
    }
}