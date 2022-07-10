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
    // Chache, um schneller Tiles anhand ihres GameObjects zu finden
    private Dictionary<GameObject, Tile> tilesByGameObject = new Dictionary<GameObject, Tile>();

    private Camera currentCamera;

    private void Awake()
    {
        SubscribeEvents();
    }

    private void Update()
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
    }

    // Caution: position has to be a world point!
    private Tile GetTileByPosition(Vector3 position) 
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

            List<Tile> neighbors = GetNeighbors(tile);
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

        UIEvents.PassMovePositionsList(movePositions);
    }

    private List<Tile> GetNeighbors(Tile tile) 
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
        return neighbors;
    }

    private void FindStartTilePositions(Character character)
    {
        List<Vector3> startTilePositions = new List<Vector3>();
        int startRow = 0;
        int endRow = boardSize/2 - 1;

        if (character.GetSide().GetPlayerType() == PlayerType.blue)
        {
            startRow = boardSize/2 + 1;
            endRow = boardSize - 1;
        }

        int row = startRow;
        while (row <= endRow)
        {
            for (int column = 0; column < boardSize; column++)
            {
                if (tilePositions[row, column] == TileType.StartTile)
                {
                    Tile tile = GetTileByCoordinates(row, column);
                    
                    if (!tile.IsOccupied())
                        startTilePositions.Add(tile.GetPosition());
                } 
            }

            row++;
        }
        
        UIEvents.PassMovePositionsList(startTilePositions);
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
        UIEvents.OnMoveOver += UpdateTilesAfterMove;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnEndDraft -= CreateBoard;
        PlacementEvents.OnCharacterSelectionForPlacement -= FindStartTilePositions;
        UIEvents.OnMoveOver -= UpdateTilesAfterMove;
    }

    #endregion
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}