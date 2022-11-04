using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Board : MonoBehaviour
{
    #region Board Config
    public static readonly int boardSize = 9;

    private static TileType[,] tilePositions = new TileType[9, 9]
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

    private static List<Tile> tiles = new List<Tile>();
    // Cache to find tiles fast, based on their gameobject
    private static Dictionary<GameObject, Tile> tilesByGameObject = new Dictionary<GameObject, Tile>();

    private void Awake()
    {
        SubscribeEvents();
    }

    public static Vector3 FindPosition(int row, int column)
    {
        return new Vector3(column * 0.7f - 3, -(row * 0.7f - 3), 1);
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

    // Caution: position has to be a world point (not a click position)!
    public static Tile GetTileByPosition(Vector3 position) 
    {
        position.z = 0;
        RaycastHit[] hits = Physics.RaycastAll(position, new Vector3(0, 0, 1));

        if (hits != null && hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                GameObject gameObject = hit.transform.gameObject;
                if (gameObject && tilesByGameObject.ContainsKey(gameObject))
                {
                    return tilesByGameObject.GetValueOrDefault(gameObject);
                }
            }
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
        List<Tile> tiles = new List<Tile>();

        if (tile.GetRow() >= distance)
        {
            tiles.Add(GetTileByCoordinates(tile.GetRow() - distance, tile.GetColumn()));
        }
        if (tile.GetRow() < boardSize - distance)
        {
            tiles.Add(GetTileByCoordinates(tile.GetRow() + distance, tile.GetColumn()));
        }
        if (tile.GetColumn() >= distance)
        {
            tiles.Add(GetTileByCoordinates(tile.GetRow(), tile.GetColumn() - distance));
        }
        if (tile.GetColumn() < boardSize - distance)
        {
            tiles.Add(GetTileByCoordinates(tile.GetRow(), tile.GetColumn() + distance));
        }

        if (patternType == PatternType.Star)
        {
            if (tile.GetRow() >= distance && tile.GetColumn() >= distance)
            {
                tiles.Add(GetTileByCoordinates(tile.GetRow() - distance, tile.GetColumn() - distance));
            }
            if (tile.GetRow() < boardSize - distance && tile.GetColumn() < boardSize - distance)
            {
                tiles.Add(GetTileByCoordinates(tile.GetRow() + distance, tile.GetColumn() + distance));
            }
            if (tile.GetRow() < boardSize - distance && tile.GetColumn() >= distance)
            {
                tiles.Add(GetTileByCoordinates(tile.GetRow() + distance, tile.GetColumn() - distance));
            }
            if (tile.GetRow() >= distance && tile.GetColumn() < boardSize - distance)
            {
                tiles.Add(GetTileByCoordinates(tile.GetRow() - distance, tile.GetColumn() + distance));
            }
        }

        return tiles;
    }

    public static List<Tile> GetAllTilesWithinRadius(Tile center, int radius)
    {
        List<Tile> tiles = new List<Tile>();
        Action<Tile> add = tile => { if (tile != null) tiles.Add(tile); };

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

    public static List<Tile> GetTilesOfNearestCharactersOfSideWithinRadius(Tile center, PlayerType side, int radius) 
    {
        List<Tile> positions = new List<Tile>();

        int i = 0;

        bool topLeft = false;
        bool top = false;
        bool topRight = false;
        bool right = false;
        bool bottomRight = false;
        bool bottom = false;
        bool bottomLeft = false;
        bool left = false;

        // Checking tiles in a star pattern to see if they are occupied.
        // Adding closest enemies to list.
        while (radius > 0)
        {
            Tile currentTile = GetTileByCoordinates(center.GetRow() - i - 1, center.GetColumn());
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide().GetPlayerType() == side && bottom == false)
            {
                positions.Add(currentTile);
                bottom = true;
            }
            currentTile = GetTileByCoordinates(center.GetRow() - i - 1, center.GetColumn() - i - 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide().GetPlayerType() == side && bottomLeft == false)
            {
                positions.Add(currentTile);
                bottomLeft = true;
            }
            currentTile = GetTileByCoordinates(center.GetRow(), center.GetColumn() - i - 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide().GetPlayerType() == side && left == false)
            {
                positions.Add(currentTile);
                left = true;
            }
            currentTile = GetTileByCoordinates(center.GetRow() - i - 1, center.GetColumn() + i + 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide().GetPlayerType() == side && topLeft == false)
            {
                positions.Add(currentTile);
                topLeft = true;
            }
            currentTile = GetTileByCoordinates(center.GetRow(), center.GetColumn() + i + 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide().GetPlayerType() == side && top == false)
            {
                positions.Add(currentTile);
                top = true;
            }
            currentTile = GetTileByCoordinates(center.GetRow() + i + 1, center.GetColumn() + i + 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide().GetPlayerType() == side && topRight == false)
            {
                positions.Add(currentTile);
                topRight = true;
            }
            currentTile = GetTileByCoordinates(center.GetRow(), center.GetColumn() + i + 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide().GetPlayerType() == side && right == false)
            {
                positions.Add(currentTile);
                right = true;
            }
            currentTile = GetTileByCoordinates(center.GetRow() - i - 1, center.GetColumn() + i + 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide().GetPlayerType() == side && bottomRight == false)
            {
                positions.Add(currentTile);
                bottomRight = true;
            }
            radius--;
            i++;
        }

        return positions;
    }

    public static List<Tile> GetAllOccupiedTilesInOneDirection(Tile startTile, Vector3 direction)
    {
        List<Tile> occupiedTiles = new List<Tile>();

        if(direction.y != 0)
        {
            if (direction.y < 0)
            {
                for (int i = startTile.GetRow() + 1; i < boardSize; i++)
                {
                    Tile tile = GetTileByCoordinates(i, startTile.GetColumn());
                    if (tile.IsOccupied()) occupiedTiles.Add(tile);
                }
            }
            else
            {
                for (int i = startTile.GetRow() - 1; i >= 0; i--)
                {
                    Tile tile = GetTileByCoordinates(i, startTile.GetColumn());
                    if (tile.IsOccupied()) occupiedTiles.Add(tile);
                }
            }
        }
        else
        {
            if(direction.x > 0)
            {
                for (int j = startTile.GetColumn() + 1; j < boardSize; j++)
                {
                    Tile tile = GetTileByCoordinates(startTile.GetRow(), j);
                    if (tile.IsOccupied()) occupiedTiles.Add(tile);
                }
            }
            else
            {
                for (int j = startTile.GetColumn() - 1; j >= 0; j--)
                {
                    Tile tile = GetTileByCoordinates(startTile.GetRow(), j);
                    if (tile.IsOccupied()) occupiedTiles.Add(tile);
                }
            }
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
                Tile tile = TileFactory.CreateTile(tilePositions[row, column], row, column);
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

    private void UpdateTilesAfterTileTransformation(Tile oldTile, Tile newTile)
    {
        tiles.Remove(oldTile);
        tiles.Add(newTile);
        tilesByGameObject.Add(newTile.GetTileGameObject(), newTile);
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        DraftEvents.OnEndDraft += CreateBoard;
        CharacterEvents.OnCharacterDeath += UpdateTileAfterCharacterDeath;
        GameplayEvents.OnTileChanged += UpdateTilesAfterTileTransformation;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnEndDraft -= CreateBoard;
        CharacterEvents.OnCharacterDeath -= UpdateTileAfterCharacterDeath;
        GameplayEvents.OnTileChanged -= UpdateTilesAfterTileTransformation;
    }

    #endregion
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}