using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile
{
    private TileType type;

    private int row;
    private int column;
    private Vector3 position;
    private GameObject tileGameObject;
    private Sprite tileSprite;
    private Character currentInhabitant;
    private State state;

    public delegate bool IsChangeable();
    public IsChangeable isChangeable;

    public Tile(TileType type, PlayerType side, int row, int column) 
    {
        this.type = type;
        this.row = row;
        this.column = column;
        this.tileSprite = TileSpriteManager.GetTileSprite(type, side, WithDepth());
        this.position = Board.FindPosition(row, column);
        this.currentInhabitant = null;
        this.isChangeable = () => !IsGoal();
        this.tileGameObject = CreateTileGameObject();
        this.state = null;
    }

    public int GetRow()
    {
        return row;
    }

    public int GetColumn()
    {
        return column;
    }

    public TileType GetTileType()
    {
        return type;
    }

    public bool IsElectrified()
    {
        return state != null && state.GetType() == typeof(ElectrifyAA) && state.IsActive();
    }

    public void SetState(TileStateType stateType)
    {
        this.state = TileStateFactory.Create(stateType, tileGameObject);
    }

    private void ResetState()
    {
        if(state != null)
            state.Destroy();

        state = null;
    }

    public Character GetCurrentInhabitant()
    {
        return currentInhabitant;
    }

    public GameObject GetTileGameObject() { return tileGameObject; }

    public Vector3 GetPosition() 
    {
        return this.position;
    }

    public bool IsGoal()
    {
        return type == TileType.GoalTile;
    }

    public bool IsHole()
    {
        return type == TileType.EmptyTile;
    }

    public bool IsNormalFloor()
    {
        return !IsHole() && !IsGoal();
    }

    public bool IsOccupied()
    {
        return currentInhabitant != null;
    }

    public bool IsAccessible() 
    {
        return !IsOccupied() && !IsHole();
    }

    public void SetCurrentInhabitant(Character character)
    {
        currentInhabitant = character;
    }

    public Tile Transform(TileType newTileType)
    {
        if (!isChangeable())
            return this;

        this.type = newTileType;
        this.tileSprite = TileSpriteManager.GetTileSprite(newTileType, Board.FindSideOfTile(row), WithDepth());
        this.tileGameObject.GetComponent<SpriteRenderer>().sprite = this.tileSprite;

        ResetState();

        // Adapt depth of tile below
        Tile tileBelow = Board.GetTileByCoordinates(GetRow() + 1, GetColumn());
        if (tileBelow != null && tileBelow.IsHole())
        {
            tileBelow.Transform(TileType.EmptyTile);
        }

        return this;
    }

    public string GetTileName()
    {
        int row = 9 - this.GetRow();
        char columnChar = (char)(this.GetColumn() + 65);
        return columnChar.ToString() + row.ToString();
    }

    private GameObject CreateTileGameObject()
    {
        GameObject tile = new GameObject
        {
            name = GetTileName()
        };

        Quaternion startRotation = Quaternion.identity;

        SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = this.tileSprite;
        spriteRenderer.sortingOrder = -2;
        tile.transform.position = position;
        tile.transform.rotation = startRotation;

        tile.AddComponent<BoxCollider>();

        tile.transform.SetParent(GameObject.Find("GameplayObjects").transform);

        return tile;
    }

    private bool WithDepth()
    {
        bool withDepth = false;
        Tile tileAbove = Board.GetTileByCoordinates(row - 1, column);
        if (tileAbove != null && !tileAbove.IsHole())
        {
            withDepth = true;
        }

        return withDepth;
    }
}