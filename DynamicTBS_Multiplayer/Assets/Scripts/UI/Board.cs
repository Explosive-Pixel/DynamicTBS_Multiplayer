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

    private Dictionary<GameObject, Tile> tilesByGameObject = new Dictionary<GameObject, Tile>();

    private Camera currentCamera;

    private void Awake()
    {
        DraftEvents.OnEndDraft += CreateBoard;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Tile tile = GetTileByPosition(Input.mousePosition);
    }

    private Tile GetTileByPosition(Vector3 position) 
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
        }

        RaycastHit hit;
        Ray ray = currentCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject gameObject = hit.transform.gameObject;
            if (gameObject && gameObject.name.Contains("Tile"))
            {
                return tilesByGameObject.GetValueOrDefault(gameObject);
            }
        }

        return null;
    }

    private void CreateBoard() 
    {
        for (int row = 0; row < boardSize; row++) 
        {
            for (int column = 0; column < boardSize; column++) 
            {
                Tile tile = TileFactory.CreateTile(tilePositions[column, row], row, column);
                tiles.Add(tile);
                tilesByGameObject.Add(tile.GetTileGameObject(), tile);
            }
        }
    }

    private void OnDestroy()
    {
        DraftEvents.OnEndDraft -= CreateBoard;
    }
}