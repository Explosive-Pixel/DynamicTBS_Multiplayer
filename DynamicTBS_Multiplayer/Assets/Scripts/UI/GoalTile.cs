using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTile : Tile
{
    public GoalTile(Vector3 position) : base(position)
    {
        this.type = TileType.GoalTile;
        this.tileSprite = SpriteManager.GOAL_TILE_SPRITE;

        Init();
    }
}