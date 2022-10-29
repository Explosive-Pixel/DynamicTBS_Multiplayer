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

   /* private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Tile tile = GetTileByPosition(currentCamera.ScreenToWorldPoint(Input.mousePosition));
    }*/

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

        List<Vector3> attackPositions = new List<Vector3>();

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
        while (range > 0)
        {
            Tile currentTile = GetTileByCoordinates(tile.GetRow() - i - 1, tile.GetColumn());
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide() != character.GetSide() && bottom == false)
            {
                attackPositions.Add(currentTile.GetPosition());
                bottom = true;
            }
            currentTile = GetTileByCoordinates(tile.GetRow() - i - 1, tile.GetColumn() - i - 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide() != character.GetSide() && bottomLeft == false)
            {
                attackPositions.Add(currentTile.GetPosition());
                bottomLeft = true;
            }
            currentTile = GetTileByCoordinates(tile.GetRow(), tile.GetColumn() - i - 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide() != character.GetSide() && left == false)
            {
                attackPositions.Add(currentTile.GetPosition());
                left = true;
            }
            currentTile = GetTileByCoordinates(tile.GetRow() - i - 1, tile.GetColumn() + i + 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide() != character.GetSide() && topLeft == false)
            {
                attackPositions.Add(currentTile.GetPosition());
                topLeft = true;
            }
            currentTile = GetTileByCoordinates(tile.GetRow(), tile.GetColumn() + i + 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide() != character.GetSide() && top == false)
            {
                attackPositions.Add(currentTile.GetPosition());
                top = true;
            }
            currentTile = GetTileByCoordinates(tile.GetRow() + i + 1, tile.GetColumn() + i + 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide() != character.GetSide() && topRight == false)
            {
                attackPositions.Add(currentTile.GetPosition());
                topRight = true;
            }
            currentTile = GetTileByCoordinates(tile.GetRow(), tile.GetColumn() + i + 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide() != character.GetSide() && right == false)
            {
                attackPositions.Add(currentTile.GetPosition());
                right = true;
            }
            currentTile = GetTileByCoordinates(tile.GetRow() - i - 1, tile.GetColumn() + i + 1);
            if (currentTile != null && currentTile.IsOccupied() && currentTile.GetCurrentInhabitant().GetSide() != character.GetSide() && bottomRight == false)
            {
                attackPositions.Add(currentTile.GetPosition());
                bottomRight = true;
            }
            range--;
            i++;
        }

        Debug.Log("Found " + attackPositions.Count + " Attack positions");

        UIEvents.PassActionPositionsList(attackPositions, UIActionType.Attack);
    }

    private List<Tile> GetNeighbors(Tile tile, PatternType patternType) 
    {
        List<Tile> neighbors = new List<Tile>();
        if (tile.GetRow() > 0) 
        {
            neighbors.Add(GetTileByCoordinates(tile.GetRow() - 1, tile.GetColumn()));
        }
        if (tile.GetRow() < boardSize-1)
        {
            neighbors.Add(GetTileByCoordinates(tile.GetRow() + 1, tile.GetColumn()));
        }
        if (tile.GetColumn() > 0)
        {
            neighbors.Add(GetTileByCoordinates(tile.GetRow(), tile.GetColumn() - 1));
        }
        if (tile.GetColumn() < boardSize-1)
        {
            neighbors.Add(GetTileByCoordinates(tile.GetRow(), tile.GetColumn() + 1));
        }

        if (patternType == PatternType.Star)
        {
            if (tile.GetRow() > 0 && tile.GetColumn() > 0)
            {
                neighbors.Add(GetTileByCoordinates(tile.GetRow() - 1, tile.GetColumn() - 1));
            }
            if (tile.GetRow() < boardSize - 1 && tile.GetColumn() < boardSize - 1)
            {
                neighbors.Add(GetTileByCoordinates(tile.GetRow() + 1, tile.GetColumn() + 1));
            }
            if (tile.GetRow() < boardSize - 1 && tile.GetColumn() > 0)
            {
                neighbors.Add(GetTileByCoordinates(tile.GetRow() + 1, tile.GetColumn() - 1));
            }
            if (tile.GetRow() > 0 && tile.GetColumn() < boardSize - 1)
            {
                neighbors.Add(GetTileByCoordinates(tile.GetRow() - 1, tile.GetColumn() + 1));
            }
        }
        
        return neighbors;
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

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        DraftEvents.OnEndDraft += CreateBoard;
        PlacementEvents.OnCharacterSelectionForPlacement += FindStartTilePositions;
        UIEvents.OnCharacterSelectionForAction += FindActionPositions;
        UIEvents.OnMoveOver += UpdateTilesAfterMove;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnEndDraft -= CreateBoard;
        PlacementEvents.OnCharacterSelectionForPlacement -= FindStartTilePositions;
        UIEvents.OnCharacterSelectionForAction -= FindActionPositions;
        UIEvents.OnMoveOver -= UpdateTilesAfterMove;
    }

    #endregion
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}