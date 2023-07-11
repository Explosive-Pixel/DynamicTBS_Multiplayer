using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

public class BoardNew : MonoBehaviour
{
    public List<Map> maps = new();

    public static MapType selectedMapType = MapType.EXPLOSIVE;
    private Map SelectedMap { get { return maps.Find(map => map.name == selectedMapType); } }

    //private List<TileMB> Tiles { get { return gameObject.GetComponentsInChildren<TileMB>().ToList(); } }
    //private List<GameObject> TileGameObjects { get { return Tiles.ConvertAll(tile => tile.gameObject); } }

    private static readonly List<GameObject> tileGameObjects = new List<GameObject>();
    public static List<GameObject> TileGameObjects { get { return tileGameObjects; } }

    public static List<TileMB> Tiles { get { return tileGameObjects.ConvertAll(go => go.GetComponent<TileMB>()); } }
    public static int Rows { get { return Tiles.Max(tile => tile.Row); } }
    public static int Columns { get { return Tiles.Max(tile => tile.Column); } }

    private void Awake()
    {
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
            TileMB tile = tileDefinition.tile.GetComponent<TileMB>();
            tile.Init(tileDefinition.tileType, tileDefinition.side);
            tileGameObjects.Add(tile.gameObject);
        }

        gameObject.SetActive(true);
    }

    public static TileMB GetTileByPosition(Vector3 position)
    {
        return UIUtils.FindGameObjectByPosition(tileGameObjects, position)?.GetComponent<TileMB>();
    }

    public static TileMB GetTileByCoordinates(int row, int column)
    {
        return Tiles.Find(t => t.Row == row && t.Column == column);
    }

    public static TileMB GetTileByCharacter(CharacterMB character)
    {
        return Tiles.Find(tile => tile.CurrentInhabitant == character);
    }

    public static bool Neighbors(TileMB tile1, TileMB tile2, PatternType patternType)
    {
        bool crossNeighbors = (tile1.Row == tile2.Row && Math.Abs(tile1.Column - tile2.Column) == 1) || (tile1.Column == tile2.Column && Math.Abs(tile1.Row - tile2.Row) == 1);

        if (patternType == PatternType.Cross)
            return crossNeighbors;

        return (crossNeighbors || (Math.Abs(tile1.Column - tile2.Column) == 1) && (Math.Abs(tile1.Row - tile2.Row) == 1));
    }

    public static List<TileMB> GetTilesOfDistance(TileMB tile, PatternType patternType, int distance)
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

    public static List<TileMB> GetAllTilesWithinRadius(TileMB center, int radius)
    {
        List<TileMB> tiles = new List<TileMB>();
        void add(TileMB tile) { if (tile != null) tiles.Add(tile); }

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

    public static List<TileMB> GetTilesOfClosestCharactersOfSideInAllStarDirections(TileMB center, PlayerType side, int distance)
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

        List<TileMB> positions = new();

        int i = 1;
        while (distance > 0)
        {
            foreach (int sig1 in signa)
            {
                foreach (int sig2 in signa)
                {
                    if (!directionFinished[(sig1, sig2)])
                    {
                        TileMB currentTile = GetTileByCoordinates(center.Row + sig1 * i, center.Column + sig2 * i);
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

    public static List<TileMB> GetTilesInAllStarDirections(TileMB center, int distance)
    {
        var signa = new[] { -1, 0, 1 };

        List<TileMB> positions = new List<TileMB>();

        int i = 1;
        while (distance > 0)
        {
            foreach (int sig1 in signa)
            {
                foreach (int sig2 in signa)
                {
                    TileMB currentTile = GetTileByCoordinates(center.Row + sig1 * i, center.Column + sig2 * i);
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

    public static List<TileMB> GetAllOccupiedTilesInOneDirection(TileMB startTile, Vector3 direction)
    {
        List<TileMB> occupiedTiles = new();

        for (int i = 1; i <= Math.Max(Rows, Columns); i++)
        {
            TileMB tile = GetTileByCoordinates(startTile.Row - (i * (int)direction.y), startTile.Column + (i * (int)direction.x));
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
