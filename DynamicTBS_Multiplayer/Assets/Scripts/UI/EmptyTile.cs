using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyTile : Tile
{

    public EmptyTile(Vector3 position) : base(position) 
    {
        this.type = TileType.EmptyTile;
    }
}
