using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterStartTile : Tile
{
    public MasterStartTile(Vector3 position) : base(position)
    {
        this.type = TileType.MasterStartTile;
        this.tileSprite = SpriteManager.MASTER_START_TILE_SPRITE;
    }
}