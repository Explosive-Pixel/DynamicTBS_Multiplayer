using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.ComponentModel;

[Serializable]
public class TileDefinition
{
    public GameObject tile;
    public TileType tileType;
    public PlayerType side;
}

[Serializable]
public class Map
{
    public MapType name;
    public List<TileDefinition> layout;
}

public enum MapType
{
    [Description("Classic")]
    EXPLOSIVE = 0,
    [Description("Bones")]
    BONES = 1,
    [Description("Arrows")]
    ARROWS = 2,
    [Description("Labyrinth")]
    LABYRINTH = 3
}

public class Board : MonoBehaviour
{
    public List<Map> maps = new();

    public static MapType selectedMapType = MapType.EXPLOSIVE;
    private Map SelectedMap { get { return maps.Find(map => map.name == selectedMapType); } }

    private static List<GameObject> tileGameObjects = new();
    public static List<GameObject> TileGameObjects { get { return tileGameObjects; } }

    public static List<Tile> Tiles = new();
    public static int Rows { get { return Tiles.Max(tile => tile.Row) + 1; } }
    public static int Columns { get { return Tiles.Max(tile => tile.Column) + 1; } }

    private void Awake()
    {
        tileGameObjects.Clear();
        Tiles.Clear();

        UnsubscribeEvents();
        SubscribeEvents();
        gameObject.SetActive(false);
        tileGameObjects.Clear();
    }

    private void InitBoard(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.DRAFT)
            return;

        foreach (TileDefinition tileDefinition in SelectedMap.layout)
        {
            Tile tile = tileDefinition.tile.GetComponent<Tile>();
            tile.Init(tileDefinition.tileType, tileDefinition.side);
        }

        Tiles = gameObject.GetComponentsInChildren<Tile>().ToList();
        tileGameObjects = Tiles.ConvertAll(tile => tile.gameObject);

        gameObject.SetActive(true);
    }

    public static Tile GetTileByPosition(Vector3 position)
    {
        GameObject tileGO = UIUtils.FindGameObjectByPosition(tileGameObjects, position);

        if (tileGO != null)
            return tileGO.GetComponent<Tile>();

        return null;
    }

    public static Tile GetTileByCoordinates(int row, int column)
    {
        Tile test = Tiles.Find(t => t.Row == row && t.Column == column);
        return test;
    }

    public static Tile GetTileByCharacter(Character character)
    {
        return Tiles.Find(tile => tile.CurrentInhabitant == character);
    }

    public static bool Neighbors(Tile tile1, Tile tile2, PatternType patternType)
    {
        bool crossNeighbors = (tile1.Row == tile2.Row && Math.Abs(tile1.Column - tile2.Column) == 1) || (tile1.Column == tile2.Column && Math.Abs(tile1.Row - tile2.Row) == 1);

        if (patternType == PatternType.Cross)
            return crossNeighbors;

        return (crossNeighbors || (Math.Abs(tile1.Column - tile2.Column) == 1) && (Math.Abs(tile1.Row - tile2.Row) == 1));
    }

    public static List<Tile> GetTilesOfDistance(Tile tile, PatternType patternType, int distance)
    {
        var tiles = (new[] {
                GetTileByCoordinates(tile.Row - distance, tile.Column),
                GetTileByCoordinates(tile.Row + distance, tile.Column),
                GetTileByCoordinates(tile.Row, tile.Column - distance),
                GetTileByCoordinates(tile.Row, tile.Column + distance)
            }
        );

        if (patternType == PatternType.Star)
        {
            tiles = tiles.Union(new[] {
                GetTileByCoordinates(tile.Row - distance, tile.Column - distance),
                GetTileByCoordinates(tile.Row + distance, tile.Column + distance),
                GetTileByCoordinates(tile.Row + distance, tile.Column - distance),
                GetTileByCoordinates(tile.Row - distance, tile.Column + distance)
            }).ToArray();
        }

        return tiles.Where(tile => tile != null).ToList();
    }

    public static List<Tile> GetAllTilesWithinRadius(Tile center, int radius)
    {
        List<Tile> tiles = new();
        void add(Tile tile) { if (tile != null) tiles.Add(tile); }

        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                if (i == 0 && j == 0) continue;

                add(GetTileByCoordinates(center.Row + i, center.Column + j));
            }
        }

        return tiles;
    }

    public static List<Tile> GetTilesOfClosestCharactersOfSideInAllStarDirections(Tile center, PlayerType side, int distance)
    {
        var signa = new[] { -1, 0, 1 };
        var directionFinished = new Dictionary<(int, int), bool>();

        foreach (int sig1 in signa)
        {
            foreach (int sig2 in signa)
            {
                directionFinished.Add((sig1, sig2), false);
            }
        }

        List<Tile> positions = new();

        int i = 1;
        while (distance > 0)
        {
            foreach (int sig1 in signa)
            {
                foreach (int sig2 in signa)
                {
                    if (!directionFinished[(sig1, sig2)])
                    {
                        Tile currentTile = GetTileByCoordinates(center.Row + sig1 * i, center.Column + sig2 * i);
                        if (currentTile != null && currentTile.IsOccupied())
                        {
                            if (currentTile.CurrentInhabitant.Side == side)
                            {
                                positions.Add(currentTile);
                            }
                            directionFinished[(sig1, sig2)] = true;
                        }
                    }
                }
            }

            i++;
            distance--;
        }

        return positions;
    }

    public static List<Tile> GetTilesInAllStarDirections(Tile center, int distance)
    {
        var signa = new[] { -1, 0, 1 };

        List<Tile> positions = new();

        int i = 1;
        while (distance > 0)
        {
            foreach (int sig1 in signa)
            {
                foreach (int sig2 in signa)
                {
                    Tile currentTile = GetTileByCoordinates(center.Row + sig1 * i, center.Column + sig2 * i);
                    if (currentTile != null)
                    {
                        positions.Add(currentTile);
                    }
                }
            }

            i++;
            distance--;
        }

        return positions;
    }

    public static List<Tile> GetAllOccupiedTilesInOneDirection(Tile startTile, Vector3 direction)
    {
        List<Tile> occupiedTiles = new();

        for (int i = 1; i <= Math.Max(Rows, Columns); i++)
        {
            Tile tile = GetTileByCoordinates(startTile.Row + (i * (int)direction.y), startTile.Column + (i * (int)direction.x));
            if (tile == null) break;
            if (tile.IsOccupied()) occupiedTiles.Add(tile);
        }

        return occupiedTiles;
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseEnd += InitBoard;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseEnd -= InitBoard;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
