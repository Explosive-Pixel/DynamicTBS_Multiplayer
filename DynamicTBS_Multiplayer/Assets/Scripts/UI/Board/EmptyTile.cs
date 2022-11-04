using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyTile : Tile
{
    public EmptyTile(int row, int column) : base(row, column) 
    {
        this.type = TileType.EmptyTile;
        this.tileSprite = SpriteManager.EMPTY_TILE_SPRITE;

        Init();
    }
}
