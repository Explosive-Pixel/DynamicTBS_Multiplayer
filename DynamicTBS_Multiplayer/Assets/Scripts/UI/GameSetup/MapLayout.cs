using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapLayout : MonoBehaviour
{
    [SerializeField] private List<Map> maps = new();

    private bool initialized = false;

    public Map GetMap(MapType mapType)
    {
        return maps.Find(map => map.name == mapType);
    }

    public void Init(MapType mapType)
    {
        Setup();

        Map map = GetMap(mapType);
        int columns = (int)Mathf.Sqrt(map.layout.Count);

        RectTransform parentRectTransform = gameObject.transform.parent.gameObject.GetComponentInChildren<RectTransform>();
        float parentHeight = GetWorldHeight(parentRectTransform);
        float tileSize = parentHeight / columns;
        Vector3 startPosition = GetBottomLeftCorner(parentRectTransform, parentRectTransform.rect.height / columns);

        int i = 0;
        int j = 0;
        foreach (TileDefinition tileDefinition in map.layout)
        {
            TilePreview tile = tileDefinition.tile.GetComponent<TilePreview>();
            tile.Init(tileDefinition.tileType, tileDefinition.side);

            tile.gameObject.GetComponentsInChildren<RectTransform>(true).ToList().ForEach(rt =>
            {
                float ratio = rt.sizeDelta.x / rt.sizeDelta.y;
                Vector2 size = new Vector2(ratio >= 1 ? tileSize : tileSize * ratio, ratio < 1 ? tileSize : tileSize * 1 / ratio);
                rt.sizeDelta = new Vector2(size.x / rt.lossyScale.x, size.y / rt.lossyScale.y);
            });
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

    private void Setup()
    {
        if (!initialized)
        {
            for (int i = 1; i < maps.Count; i++)
            {
                Map map = maps[i];
                int j = 0;
                foreach (TileDefinition td in map.layout)
                {
                    td.tile = maps[0].layout[j].tile;
                    j++;
                }
            }

            initialized = true;
        }
    }

    public void SetActive(bool active)
    {
        BaseActiveHandler activeHandler = gameObject.GetComponentInParent<BaseActiveHandler>(true);

        if (activeHandler == null)
        {
            gameObject.SetActive(active);
            return;
        }

        if (active && !gameObject.activeSelf)
            activeHandler.SetActive(gameObject);
        else if (!active && gameObject.activeSelf)
            activeHandler.SetInactive();

        if (activeHandler.gameObject.activeSelf != active)
            activeHandler.gameObject.SetActive(active);
    }

    private Vector3 GetBottomLeftCorner(RectTransform rectTransform, float tileSize)
    {
        Vector2 localBottomLeft = new(
            rectTransform.rect.xMin + tileSize / 2,
            rectTransform.rect.yMin + tileSize / 2
        );

        Vector3 worldBottomLeft = rectTransform.TransformPoint(localBottomLeft);
        return worldBottomLeft;
    }

    private float GetWorldHeight(RectTransform rectTransform)
    {
        Vector3 bottom = rectTransform.TransformPoint(new Vector3(0, rectTransform.rect.yMin, 0));
        Vector3 top = rectTransform.TransformPoint(new Vector3(0, rectTransform.rect.yMax, 0));

        float worldHeight = Vector3.Distance(bottom, top);
        return worldHeight;
    }
}
