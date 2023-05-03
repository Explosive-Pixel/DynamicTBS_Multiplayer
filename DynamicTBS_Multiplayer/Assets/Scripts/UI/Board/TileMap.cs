using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TileMap
{
    [SerializeField] private List<TileType> tiles = new List<TileType>(new TileType[Board.boardSize*Board.boardSize]);

    public TileMap()
    {

    }

    public TileType this[int x, int y]
    {
        get
        {
            return tiles[x * Board.boardSize + y];
        }
        set
        {
            tiles[x * Board.boardSize + y] = value;
        }
    }
}