using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tile
{
    protected TileType type;

    protected Vector3 position;
    protected GameObject tileGameObject;
    protected Sprite tileSprite;
    protected Character currentInhabitant;

    protected Tile(Vector3 position) 
    {
        Debug.Log("TileConstructor called.");
        this.position = position;
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
        
        Quaternion startRotation = Quaternion.identity;

        SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = this.tileSprite;
        tile.transform.position = position;
        tile.transform.rotation = startRotation;

        return tile;
    }
}