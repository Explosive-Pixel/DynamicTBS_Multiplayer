using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Board : MonoBehaviour
{
    #region Board Config
    private static readonly int boardSize = 9;

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

    private List<Tile> tiles = new List<Tile>();
    // Cache to find tiles fast, based on their gameobject
    private Dictionary<GameObject, Tile> tilesByGameObject = new Dictionary<GameObject, Tile>();

    private Camera currentCamera;

    private void Awake()
    {
        SubscribeEvents();
    }

    // Caution: position has to be a world point!
    public Tile GetTileByPosition(Vector3 position) 
    {
        position.z = 0;
        RaycastHit[] hits = Physics.RaycastAll(position, transform.forward);

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

    public List<Tile> GetTilesOfDistance(Tile tile, PatternType patternType, int distance)
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

    public List<Tile> GetAllTilesWithinRadius(Tile center, int radius)
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

    public List<Tile> GetTilesOfNearestCharactersOfSideWithinRadius(Tile center, PlayerType side, int radius) 
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

    private Tile GetTileByCharacter(Character character) 
    {
        return GetTileByPosition(character.GetCharacterGameObject().transform.position);
    }

    private Tile GetTileByCoordinates(int row, int column)
    {
        if (row < 0 || row >= boardSize || column < 0 || column >= boardSize) {
            return null;
        }

        return tiles.Find(t => t.GetRow() == row && t.GetColumn() == column);
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

    private void FindActionPositions(Character character, UIActionType type)
    {
        switch (type)
        {
            case UIActionType.Move:
                {
                    FindMovePositions(character);
                    break;
                }
            case UIActionType.Attack:
                {
                    FindAttackPositions(character);
                    break;
                }
        }
    }

    private void FindMovePositions(Character character) 
    {   
        Tile currentTile = GetTileByCharacter(character);

        if (currentTile == null) return;

        List<Vector3> movePositions = new List<Vector3>();

        int range = character.GetMoveSpeed();
        Queue<Tile> queue = new Queue<Tile>();
        List<Tile> visited = new List<Tile>();
        queue.Enqueue(currentTile);
        while (queue.Count > 0 && range > 0)
        {
            Tile tile = queue.Dequeue();
            visited.Add(tile);

            List<Tile> neighbors = GetNeighbors(tile, PatternType.Cross);
            foreach(Tile neighbor in neighbors) 
            {
                if (!visited.Contains(neighbor) && neighbor.IsAccessible()) 
                {
                    movePositions.Add(neighbor.GetTileGameObject().transform.position);
                    queue.Enqueue(neighbor);
                }
            }
            range--;
        }

        UIEvents.PassActionPositionsList(movePositions, UIActionType.Move);
    }

    private void FindAttackPositions(Character character)
    {   
        int range = character.GetAttackRange();
        Tile tile = GetTileByCharacter(character);

        PlayerType otherSide = character.GetSide().GetPlayerType() == PlayerType.blue ? PlayerType.pink : PlayerType.blue;

        List<Tile> attackTiles = GetTilesOfNearestCharactersOfSideWithinRadius(tile, otherSide, range)
            .FindAll(tile => tile.GetCurrentInhabitant() != null && tile.GetCurrentInhabitant().CanReceiveDamage());

        List<Vector3> attackPositions = attackTiles.ConvertAll(tile => tile.GetPosition());

        UIEvents.PassActionPositionsList(attackPositions, UIActionType.Attack);
    }

    private List<Tile> GetNeighbors(Tile tile, PatternType patternType) 
    {
        return GetTilesOfDistance(tile, patternType, 1);
    }

    private void FindStartTilePositions(Character character)
    {
        List<Vector3> startTilePositions = FindStartTiles(character).ConvertAll(tile => tile.GetPosition());
        
        UIEvents.PassActionPositionsList(startTilePositions, UIActionType.Move);
    }

    public List<Tile> FindStartTiles(Character character) 
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

    public Tile FindMasterStartTile(PlayerType playerType) 
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

    private int FindStartRow(PlayerType side) 
    {
        return side == PlayerType.blue ? (boardSize / 2 + 1) : 0;
    }

    private int FindEndRow(PlayerType side)
    {
        return side == PlayerType.blue ? (boardSize - 1) : (boardSize / 2 - 1);
    }

    private void UpdateTilesAfterMove(Vector3 previousTile, Character character)
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
        PlacementEvents.OnCharacterSelectionForPlacement += FindStartTilePositions;
        UIEvents.OnCharacterSelectionForAction += FindActionPositions;
        UIEvents.OnMoveOver += UpdateTilesAfterMove;
        CharacterEvents.OnCharacterDeath += UpdateTileAfterCharacterDeath;
        GameplayEvents.OnTileChanged += UpdateTilesAfterTileTransformation;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnEndDraft -= CreateBoard;
        PlacementEvents.OnCharacterSelectionForPlacement -= FindStartTilePositions;
        UIEvents.OnCharacterSelectionForAction -= FindActionPositions;
        UIEvents.OnMoveOver -= UpdateTilesAfterMove;
        CharacterEvents.OnCharacterDeath -= UpdateTileAfterCharacterDeath;
        GameplayEvents.OnTileChanged -= UpdateTilesAfterTileTransformation;
    }

    #endregion
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}