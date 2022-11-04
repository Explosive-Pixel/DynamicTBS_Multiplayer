using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTile : Tile
{
    public StartTile(int row, int column) : base(row, column)
    {
        this.type = TileType.StartTile;
        this.tileSprite = SpriteManager.START_TILE_SPRITE;

        Init();
    }
}