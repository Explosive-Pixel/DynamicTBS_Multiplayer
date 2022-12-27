using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class Board : MonoBehaviour
{
    #region Board Config

    public const int boardSize = 9;
    public const float startTranslation = 3;

    private const float tileSize = 0.7f;

    private static readonly TileType[,] tilePositions = new TileType[boardSize, boardSize]
    {
        { TileType.FloorTile, TileType.FloorTile, TileType.MasterStartTile, TileType.FloorTile, TileType.FloorTile, TileType.FloorTile, TileType.FloorTile, TileType.FloorTile, TileType.FloorTile },
        { TileType.EmptyTile, TileType.EmptyTile, TileType.FloorTile, TileType.StartTile, TileType.EmptyTile, TileType.StartTile, TileType.FloorTile, TileType.EmptyTile, TileType.EmptyTile },
        { TileType.StartTile, TileType.FloorTile, TileType.FloorTile, TileType.EmptyTile, TileType.StartTile, TileType.EmptyTile, TileType.FloorTile, TileType.FloorTile, TileType.StartTile },
        { TileType.FloorTile, TileType.EmptyTile, TileType.StartTile, TileType.FloorTile, TileType.EmptyTile, TileType.FloorTile, TileType.StartTile, TileType.EmptyTile, TileType.FloorTile },
        { TileType.FloorTile, TileType.EmptyTile, TileType.EmptyTile, TileType.FloorTile, TileType.GoalTile, TileType.FloorTile, TileType.EmptyTile, TileType.EmptyTile, TileType.FloorTile },
        { TileType.FloorTile, TileType.EmptyTile, TileType.StartTile, TileType.FloorTile, TileType.EmptyTile, TileType.FloorTile, TileType.StartTile, TileType.EmptyTile, TileType.FloorTile },
        { TileType.StartTile, TileType.FloorTile, TileType.FloorTile, TileType.EmptyTile, TileType.StartTile, TileType.EmptyTile, TileType.FloorTile, TileType.FloorTile, TileType.StartTile },
        { TileType.EmptyTile, TileType.EmptyTile, TileType.FloorTile, TileType.StartTile, TileType.EmptyTile, TileType.StartTile, TileType.FloorTile, TileType.EmptyTile, TileType.EmptyTile },
        { TileType.FloorTile, TileType.FloorTile, TileType.FloorTile, TileType.FloorTile, TileType.FloorTile, TileType.FloorTile, TileType.MasterStartTile, TileType.FloorTile, TileType.FloorTile }
    };

    #endregion

    private static readonly List<Tile> tiles = new List<Tile>();

    // Cache to find tiles fast, based on their gameobject
    private static readonly Dictionary<GameObject, Tile> tilesByGameObject = new Dictionary<GameObject, Tile>();

    private void Awake()
    {
        SubscribeEvents();
    }

    public static Vector3 FindPosition(int row, int column)
    {
        return new Vector3(column * tileSize - startTranslation, -(row * tileSize - startTranslation), 1);
    }

    public static List<Tile> GetAllTiles()
    {
        return tiles;
    }

    public static Tile GetTileByGameObject(GameObject gameObject)
    {
        return tilesByGameObject[gameObject];
    }

    public static Tile GetTileByCharacter(Character character)
    {
        if(character.GetCharacterGameObject() != null)
            return GetTileByPosition(character.GetCharacterGameObject().transform.position);
        return null;
    }

    public static Tile GetTileByPosition(Vector3 position) 
    {
        GameObject gameObject = UIUtils.FindGameObjectByPosition(tilesByGameObject.Keys.ToList(), position);

        if (gameObject && tilesByGameObject.ContainsKey(gameObject))
        {
            return tilesByGameObject.GetValueOrDefault(gameObject);
        }

        return null;
    }

    public static bool Neighbors(Tile tile1, Tile tile2, PatternType patternType)
    {
        bool crossNeighbors = (tile1.GetRow() == tile2.GetRow() && Math.Abs(tile1.GetColumn() - tile2.GetColumn()) == 1) || (tile1.GetColumn() == tile2.GetColumn() && Math.Abs(tile1.GetRow() - tile2.GetRow()) == 1);

        if (patternType == PatternType.Cross)
            return crossNeighbors;

        return (crossNeighbors || (Math.Abs(tile1.GetColumn() - tile2.GetColumn()) == 1) && (Math.Abs(tile1.GetRow() - tile2.GetRow()) == 1));
    }

    public static List<Tile> GetTilesOfDistance(Tile tile, PatternType patternType, int distance)
    {
        var tiles = (new[] {
                GetTileByCoordinates(tile.GetRow() - distance, tile.GetColumn()),
                GetTileByCoordinates(tile.GetRow() + distance, tile.GetColumn()),
                GetTileByCoordinates(tile.GetRow(), tile.GetColumn() - distance),
                GetTileByCoordinates(tile.GetRow(), tile.GetColumn() + distance)
            }
        );
        
        if(patternType == PatternType.Star)
        {
            tiles = tiles.Union(new[] {
                GetTileByCoordinates(tile.GetRow() - distance, tile.GetColumn() - distance),
                GetTileByCoordinates(tile.GetRow() + distance, tile.GetColumn() + distance),
                GetTileByCoordinates(tile.GetRow() + distance, tile.GetColumn() - distance),
                GetTileByCoordinates(tile.GetRow() - distance, tile.GetColumn() + distance)
            }).ToArray();
        }
        
        return tiles.Where(tile => tile != null).ToList();
    }

    public static List<Tile> GetAllTilesWithinRadius(Tile center, int radius)
    {
        List<Tile> tiles = new List<Tile>();
        void add(Tile tile) { if (tile != null) tiles.Add(tile); }

        for (int i = -radius; i <= radius; i++) 
        {
            for (int j = -radius; j <= radius; j++) 
            {
                if (i == 0 && j == 0) continue;

                add(GetTileByCoordinates(center.GetRow() + i, center.GetColumn() + j));
            }
        }

        return tiles;
    }

    public static List<Tile> GetTilesOfClosestCharactersOfSideWithinRadius(Tile center, PlayerType side, int radius)
    {
        var signa = new [] { -1, 0, 1 };
        var directionFinished = new Dictionary<(int, int), bool>();

        foreach(int sig1 in signa)
        {
            foreach(int sig2 in signa)
            {
                directionFinished.Add((sig1, sig2), false);
            }
        }

        List<Tile> positions = new List<Tile>();

        int i = 1;
        while(radius > 0)
        {
            foreach (int sig1 in signa)
            {
                foreach (int sig2 in signa)
                {
                    if(!directionFinished[(sig1, sig2)])
                    {
                        Tile currentTile = GetTileByCoordinates(center.GetRow() + sig1*i, center.GetColumn() + sig2*i);
                        if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide().GetPlayerType() == side)
                        {
                            positions.Add(currentTile);
                            directionFinished[(sig1, sig2)] = true;
                        }
                    }
                }
            }

            i++;
            radius--;
        }

        return positions;
    }

    public static List<Tile> GetAllOccupiedTilesInOneDirection(Tile startTile, Vector3 direction)
    {
        List<Tile> occupiedTiles = new List<Tile>();

        for(int i = 1; i < boardSize; i++)
        {
            Tile tile = GetTileByCoordinates(startTile.GetRow() - (i * (int)direction.y), startTile.GetColumn() + (i * (int)direction.x));
            if (tile == null) break;
            if (tile.IsOccupied()) occupiedTiles.Add(tile);
        }

        return occupiedTiles;
    }

    public static void UpdateTilesAfterMove(Vector3 previousTile, Character character)
    {
        Tile tile = GetTileByPosition(previousTile);

        if (tile != null)
        {
            tile.SetCurrentInhabitant(null);
        }

        tile = GetTileByCharacter(character);

        if (tile != null)
        {
            tile.SetCurrentInhabitant(character);
        }
    }

    private static Tile GetTileByCoordinates(int row, int column)
    {
        if (row < 0 || row >= boardSize || column < 0 || column >= boardSize) {
            return null;
        }

        return tiles.Find(t => t.GetRow() == row && t.GetColumn() == column);
    }

    public static List<Tile> GetNeighbors(Tile tile, PatternType patternType) 
    {
        return GetTilesOfDistance(tile, patternType, 1);
    }

    public static List<Tile> FindStartTiles(Character character) 
    {
        List<Tile> startTiles = new List<Tile>();
        int startRow = FindStartRow(character.GetSide().GetPlayerType());
        int endRow = FindEndRow(character.GetSide().GetPlayerType());

        int row = startRow;
        while (row <= endRow)
        {
            for (int column = 0; column < boardSize; column++)
            {
                if (tilePositions[row, column] == TileType.StartTile)
                {
                    Tile tile = GetTileByCoordinates(row, column);

                    if (!tile.IsOccupied())
                        startTiles.Add(tile);
                }
            }

            row++;
        }

        return startTiles;
    }

    public static Tile FindMasterStartTile(PlayerType playerType) 
    {
        int startRow = FindStartRow(playerType);
        int endRow = FindEndRow(playerType);

        int row = startRow;
        while (row <= endRow)
        {
            for (int column = 0; column < boardSize; column++)
            {
                if (tilePositions[row, column] == TileType.MasterStartTile)
                {
                    return GetTileByCoordinates(row, column);
                }
            }

            row++;
        }

        return null;
    }

    private static int FindStartRow(PlayerType side) 
    {
        return side == PlayerType.blue ? (boardSize / 2 + 1) : 0;
    }

    private static int FindEndRow(PlayerType side)
    {
        return side == PlayerType.blue ? (boardSize - 1) : (boardSize / 2 - 1);
    }

    private void CreateBoard()
    {
        for (int row = 0; row < boardSize; row++)
        {
            for (int column = 0; column < boardSize; column++)
            {
                Tile tile = new Tile(tilePositions[row, column], row, column);
                tiles.Add(tile);
                tilesByGameObject.Add(tile.GetTileGameObject(), tile);
            }
        }
    }

    private void UpdateTileAfterCharacterDeath(Character character, Vector3 position)
    {
        Tile tile = GetTileByPosition(position);

        if (tile != null)
        {
            tile.SetCurrentInhabitant(null);
        }
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        DraftEvents.OnEndDraft += CreateBoard;
        CharacterEvents.OnCharacterDeath += UpdateTileAfterCharacterDeath;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnEndDraft -= CreateBoard;
        CharacterEvents.OnCharacterDeath -= UpdateTileAfterCharacterDeath;
    }

    #endregion
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}