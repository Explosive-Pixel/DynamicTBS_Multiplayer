using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class TileDefinition
{
    public GameObject tile;
    public TileType tileType;
    public PlayerType side;
}

[Serializable]
public class Map
{
    public MapType name;
    public List<TileDefinition> layout;
}

public class BoardNew : MonoBehaviour
{
    public List<Map> maps = new();

    public static MapType selectedMapType = MapType.EXPLOSIVE;
    private Map SelectedMap { get { return maps.Find(map => map.name == selectedMapType); } }

    //private List<TileMB> Tiles { get { return gameObject.GetComponentsInChildren<TileMB>().ToList(); } }
    //private List<GameObject> TileGameObjects { get { return Tiles.ConvertAll(tile => tile.gameObject); } }
    //private int Rows { get { return Tiles.Max(tile => tile.row); } }
    //private int Columns { get { return Tiles.Max(tile => tile.column); } }

    private static readonly List<GameObject> tileGameObjects = new List<GameObject>();
    public static List<GameObject> TileGameObjects { get { return tileGameObjects; } }

    private void Awake()
    {
        UnsubscribeEvents();
        SubscribeEvents();
        gameObject.SetActive(false);
        tileGameObjects.Clear();
    }

    private void InitBoard(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.DRAFT)
            return;

        foreach (TileDefinition tileDefinition in SelectedMap.layout)
        {
            TileMB tile = tileDefinition.tile.GetComponent<TileMB>();
            tile.Init(tileDefinition.tileType, tileDefinition.side);
            tileGameObjects.Add(tile.gameObject);
        }

        gameObject.SetActive(true);
    }

    public static GameObject GetTileByPosition(Vector3 position)
    {
        return UIUtils.FindGameObjectByPosition(tileGameObjects, position);
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        //GameEvents.OnGamePhaseEnd += InitBoard;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseEnd -= InitBoard;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
