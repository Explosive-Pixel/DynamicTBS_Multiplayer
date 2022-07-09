using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : Tile
{
    public FloorTile(int row, int column) : base(row, column)
    {
        this.type = TileType.FloorTile;
        this.tileSprite = SpriteManager.FLOOR_TILE_SPRITE;
        
        Init();
    }
}