using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterStartTile : Tile
{
    public MasterStartTile(int row, int column) : base(row, column)
    {
        this.type = TileType.MasterStartTile;
        this.tileSprite = SpriteManager.MASTER_START_TILE_SPRITE;
        this.isChangeable = () => false;

        Init();
    }
}