using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    #region Helper classes
    private class Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int X, int Y) 
        {
            this.X = X;
            this.Y = Y;
        }

        public bool HasType(TileType type) 
        {
            return tilePositions.GetValueOrDefault(type).Find(p => p.Equals(this)) != null;
        }

        public override bool Equals(System.Object obj) 
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Point p = (Point)obj;
                return (this.X == p.X) && (this.Y == p.Y);
            }
        }

        public override int GetHashCode()
        {
            return this.X * boardSize + this.Y;
        }
    }
    #endregion

    #region Board Config
    private static readonly int boardSize = 9;

    private static readonly Dictionary<TileType, List<Point>> tilePositions = new Dictionary<TileType, List<Point>> {
        { TileType.EmptyTile, new List<Point>{ new Point(1,0) } }, // TODO
        { TileType.FloorTile, new List<Point>{ new Point(0,0), new Point(0,1) } }, // TODO
        { TileType.GoalTile, new List<Point>{ new Point(4,4) } },
        { TileType.StartTile, new List<Point>{ new Point(1,3) } }, // TODO
        { TileType.MasterStartTile, new List<Point>{ new Point(0,2), new Point(8,6) } }
    };
    #endregion

    private List<Tile> tiles = new List<Tile>();

    // Cache um Tiles schneller anhand der Position zu finden
    private readonly Dictionary<Vector3, Tile> tilesByPosition = new Dictionary<Vector3, Tile>();

    public Board()
    {
        CreateBoard();
    }

    public Tile GetTile(Vector3 position) {
        return tilesByPosition.GetValueOrDefault(position, null);
    }

    private void CreateBoard() 
    {
        for (int row = 0; row < boardSize; row++) 
        {
            for (int column = 0; column < boardSize; column++) 
            {
                Point point = new Point(row, column);
                if (point.HasType(TileType.EmptyTile))
                {
                    tiles.Add(new EmptyTile(GetPosition(point)));
                }
                else if (point.HasType(TileType.FloorTile))
                {
                    tiles.Add(new FloorTile(GetPosition(point)));
                }
                else if (point.HasType(TileType.StartTile))
                {
                    tiles.Add(new StartTile(GetPosition(point)));
                }
                else if (point.HasType(TileType.MasterStartTile))
                {
                    tiles.Add(new MasterStartTile(GetPosition(point)));
                }
                else if (point.HasType(TileType.GoalTile))
                {
                    tiles.Add(new GoalTile(GetPosition(point)));
                }
            }
        }

        foreach (Tile tile in tiles) {
            tilesByPosition.Add(tile.GetPosition(), tile);
        }
    }

    // TODO
    private Vector3 GetPosition(Point p) 
    {
        return new Vector3(p.X, p.Y);
    }
}