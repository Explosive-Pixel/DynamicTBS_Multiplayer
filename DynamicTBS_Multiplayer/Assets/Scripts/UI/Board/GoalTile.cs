using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTile : Tile
{
    public GoalTile(int row, int column) : base(row, column)
    {
        this.type = TileType.GoalTile;
        this.tileSprite = SpriteManager.GOAL_TILE_SPRITE;

        Init();
    }
}