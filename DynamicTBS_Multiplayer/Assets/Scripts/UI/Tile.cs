using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] protected GameObject tilePrefab;

    protected Vector3 position;
    protected Character currentInhabitant { get; set; }

    protected Tile(Vector3 position) 
    {
        this.position = position;
        this.currentInhabitant = null;
    }
}