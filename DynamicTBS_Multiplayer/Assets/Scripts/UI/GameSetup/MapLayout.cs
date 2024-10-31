using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapLayout : MonoBehaviour
{
    [SerializeField] private List<Map> maps = new();
    [SerializeField] private float tileSize = 40;
    [SerializeField] private Vector3 startPosition = new(0f, 0f, 0f);

    public Map GetMap(MapType mapType)
    {
        return maps.Find(map => map.name == mapType);
    }

    public void Init(MapType mapType)
    {
        Map map = GetMap(mapType);

        int i = 0;
        int j = 0;
        int columns = (int)Mathf.Sqrt(map.layout.Count);
        foreach (TileDefinition tileDefinition in map.layout)
        {
            TilePreview tile = tileDefinition.tile.GetComponent<TilePreview>();
            tile.Init(tileDefinition.tileType, tileDefinition.side);

            tile.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize, tileSize);
            tile.gameObject.GetComponentsInChildren<RectTransform>(true).ToList().ForEach(rt => rt.sizeDelta = new Vector2(tileSize, tileSize));
            tile.gameObject.transform.position = new Vector3(startPosition.x + i * tileSize, startPosition.y + j * tileSize, tile.gameObject.transform.position.z);
            j++;
            if (j == columns)
            {
                i++;
                j = 0;
            }
        }

        SetActive(true);
    }

    public void SetActive(bool active)
    {
        BaseActiveHandler activeHandler = gameObject.GetComponentInParent<BaseActiveHandler>(true);

        if (active && !gameObject.activeSelf)
            activeHandler.SetActive(gameObject);
        else if (!active && gameObject.activeSelf)
            activeHandler.SetInactive();

        if (activeHandler.gameObject.activeSelf != active)
            activeHandler.gameObject.SetActive(active);
    }
}
