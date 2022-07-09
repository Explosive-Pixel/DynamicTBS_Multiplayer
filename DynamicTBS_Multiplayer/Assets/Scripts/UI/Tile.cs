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
    protected Character currentInhabitant { get; set; }

    protected Tile(Vector3 position) 
    {
        this.position = position;
        this.tileGameObject = CreateTileGameObject(position);
        this.currentInhabitant = null;
    }

    public GameObject GetTileGameObject() { return tileGameObject; }

    public Vector3 GetPosition() 
    {
        return this.position;
    }

    private GameObject CreateTileGameObject(Vector3 position)
    {
        GameObject tile = new GameObject();

        Vector3 startScale = new Vector3(100, 0, 100);
        Quaternion startRotation = Quaternion.identity;

        SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = this.tileSprite;
        tile.transform.localScale = startScale;
        tile.transform.position = position;
        tile.transform.rotation = startRotation;

        return tile;
    }
}