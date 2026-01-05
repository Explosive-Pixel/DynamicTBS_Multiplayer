using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

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
    LABYRINTH = 3,
    [Description("Radiance")]
    RADIANCE = 4,
    [Description("Fracture")]
    FRACTURE = 5
}

public class Board : MonoBehaviour
{
    public List<Map> maps = new();

    public static MapType SelectedMapType { get { return GameSetup.SetupCompleted ? GameSetup.MapSetup.MapType : MapType.EXPLOSIVE; } }
    private Map SelectedMap { get { return maps.Find(map => map.name == SelectedMapType); } }

    private static List<GameObject> tileGameObjects = new();
    public static List<GameObject> TileGameObjects { get { return tileGameObjects; } }

    public static List<Tile> Tiles = new();
    public static int Rows { get { return Tiles.Max(tile => tile.Row) + 1; } }
    public static int Columns { get { return Tiles.Max(tile => tile.Column) + 1; } }

    private void Awake()
    {
        tileGameObjects.Clear();
        Tiles.Clear();

        SubscribeEvents();

        tileGameObjects.Clear();
    }

    public static void InitBoard(List<Tile> tiles)
    {
        Tiles = tiles;
        tileGameObjects = Tiles.ConvertAll(tile => tile.gameObject);
    }

    private void InitBoard(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.PLACEMENT)
            return;

        foreach (TileDefinition tileDefinition in SelectedMap.layout)
        {
            Tile tile = tileDefinition.tile.GetComponent<Tile>();
            tile.Init(tileDefinition.tileType, tileDefinition.side);
        }

        Tiles = gameObject.GetComponentsInChildren<Tile>().ToList();
        tileGameObjects = Tiles.ConvertAll(tile => tile.gameObject);

        /*
         * TODO
         * only for a transitional purpose
         * please delete as soon as possible or refactor
         */
        Camera.main.orthographicSize = 1.63f;
        GameObject.Find("Background").transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        /*
         * 
         */

        GameplayEvents.GameplayUISetupFinished();
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

    /// <summary>
    /// Checks whether two tiles are adjacent with respect to a given pattern type.
    /// </summary>
    public static bool Neighbors(Tile tile1, Tile tile2, PatternType patternType)
    {
        if (tile1 == null || tile2 == null)
            return false;

        bool crossNeighbors = (tile1.Row == tile2.Row && Math.Abs(tile1.Column - tile2.Column) == 1) || (tile1.Column == tile2.Column && Math.Abs(tile1.Row - tile2.Row) == 1);

        if (patternType == PatternType.Cross)
            return crossNeighbors;

        return (crossNeighbors || (Math.Abs(tile1.Column - tile2.Column) == 1) && (Math.Abs(tile1.Row - tile2.Row) == 1));
    }

    /// <summary>
    /// Returns all tiles having a given distance from a given tile.
    /// </summary>
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

    /// <summary>
    /// Returns all occupied tiles in one direction (beginning from startTile, ending at the end of the board).
    /// </summary>
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

    /// <summary>
    /// Returns all tiles within a distance to a given tile (with respect to a given pattern type).
    /// </summary>
    public static List<Tile> GetTilesInAllDirections(Tile center, PatternType patternType, int distance)
    {
        return GetCustomTilesInAllDirections(center, patternType, distance,
            (currentTile, positions, directionFinished, sig1, sig2) =>
            {
                positions.Add(currentTile);
            });
    }

    /// <summary>
    /// Returns all occupied tiles within a distance to a given tile (with respect to a given pattern type).
    /// If side is specified (i.e. side != none), only occupied tiles with inhabitants of the given side are returned.
    /// </summary>
    public static List<Tile> GetTilesOfClosestCharactersOfSideInAllDirections(Tile center, PlayerType side, PatternType patternType, int distance)
    {
        return GetCustomTilesInAllDirections(center, patternType, distance,
            (currentTile, positions, directionFinished, sig1, sig2) =>
            {
                if (currentTile.IsOccupied())
                {
                    if (side == PlayerType.none || currentTile.CurrentInhabitant.Side == side)
                    {
                        positions.Add(currentTile);
                    }
                    directionFinished[(sig1, sig2)] = true;
                }
            });

    }

    /// <summary>
    /// Returns all tiles within a distance to a given tile (with respect to a given pattern type) until - for each direction - a tile is occupied.
    /// The respective first occupied tiles are also included in the returned list.
    /// </summary>
    public static List<Tile> GetTilesUntilClosestCharactersInAllDirections(Tile center, PatternType patternType, int distance)
    {
        return GetCustomTilesInAllDirections(center, patternType, distance,
            (currentTile, positions, directionFinished, sig1, sig2) =>
            {
                positions.Add(currentTile);

                if (currentTile.IsOccupied())
                {
                    directionFinished[(sig1, sig2)] = true;
                }
            });
    }

    public static Tile GetNextTileInSameDirection(Tile tile, Tile next)
    {
        int rowDiff = next.Row - tile.Row;
        int columnDiff = next.Column - tile.Column;

        return GetTileByCoordinates(next.Row + rowDiff, next.Column + columnDiff);
    }

    private static List<Tile> GetCustomTilesInAllDirections(Tile center, PatternType patternType, int distance, Action<Tile, List<Tile>, Dictionary<(int, int), bool>, int, int> customAction)
    {
        var signa = new[] { -1, 0, 1 };
        var directionFinished = new Dictionary<(int, int), bool>();

        foreach (int sig1 in signa)
        {
            foreach (int sig2 in signa)
            {
                if (patternType == PatternType.Cross && sig1 != 0 && sig2 != 0)
                    continue;

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
                    if (patternType == PatternType.Cross && sig1 != 0 && sig2 != 0)
                        continue;

                    if (!directionFinished[(sig1, sig2)])
                    {
                        Tile currentTile = GetTileByCoordinates(center.Row + sig1 * i, center.Column + sig2 * i);
                        if (currentTile != null)
                        {
                            customAction(currentTile, positions, directionFinished, sig1, sig2);
                        }
                    }
                }
            }

            i++;
            distance--;
        }

        return positions;
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += InitBoard;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= InitBoard;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
