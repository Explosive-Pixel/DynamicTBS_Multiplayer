using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tile
{
    protected TileType type;

    protected int row;
    public int Row { get; }
    protected int column;
    public int Column { get; }
    protected Vector3 position;
    protected GameObject tileGameObject;
    protected Sprite tileSprite;
    protected Character currentInhabitant;

    protected Tile(int row, int column) 
    {
        this.row = row;
        this.column = column;
        this.position = FindPosition(row, column);
        this.currentInhabitant = null;
    }

    protected void Init()
    {
        this.tileGameObject = CreateTileGameObject();
    }

    public GameObject GetTileGameObject() { return tileGameObject; }

    public Vector3 GetPosition() 
    {
        return this.position;
    }

    private GameObject CreateTileGameObject()
    {
        GameObject tile = new GameObject();

        tile.name = this.GetType().Name;
        
        Quaternion startRotation = Quaternion.identity;

        SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = this.tileSprite;
        tile.transform.position = position;
        tile.transform.rotation = startRotation;

        tile.AddComponent<BoxCollider>();

        return tile;
    }
    private Vector3 FindPosition(int x, int y)
    {
        return new Vector3(Convert.ToSingle(x * 0.7) - 3, Convert.ToSingle(y * 0.7) - 3, 1);
    }
}