using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTile : Tile
{
    public StartTile(Vector3 position) : base(position)
    {
        this.type = TileType.StartTile;
    }
}