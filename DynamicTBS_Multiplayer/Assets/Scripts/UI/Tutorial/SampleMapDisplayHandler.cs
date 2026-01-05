using System.Collections.Generic;
using UnityEngine;

public class SampleMapDisplayHandler : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private MapLayout mapPreview;
    [SerializeField] private MapType mapType = MapType.EXPLOSIVE;

    private void Awake()
    {
        Map map = mapPreview.GetMap(mapType);
        InitBoard(ToBoardLayout(map), tilePrefab);
    }

    private void InitBoard(BoardLayout layout, GameObject tilePrefab)
    {
        float startX = layout.startX;
        float startY = layout.startY;

        float size = tilePrefab
            .GetComponent<SpriteRenderer>()
            .sprite.bounds.size.x;

        int rowIndex = 0;

        foreach (RowLayout row in layout.rows)
        {
            int columnIndex = 0;

            foreach (TileLayout tileLayout in row.tiles)
            {
                GameObject tileGO = Instantiate(tilePrefab);
                tileGO.SetActive(true);

                tileGO.transform.SetParent(gameObject.transform, false);

                tileGO.transform.localPosition = new Vector3(
                    startX + columnIndex * size,
                    startY + rowIndex * size,
                    1
                );

                tileGO.transform.localScale = Vector3.one;

                Tile tile = tileGO.GetComponent<Tile>();
                tile.Init(tileLayout.tileType, tileLayout.side, rowIndex, columnIndex);

                columnIndex++;
            }

            rowIndex++;
        }
    }

    private BoardLayout ToBoardLayout(Map map)
    {
        int size = (int)Mathf.Sqrt(map.layout.Count);
        List<RowLayout> rows = new(size);

        for (int row = 0; row < size; row++)
        {
            List<TileLayout> tiles = new(size);

            for (int col = 0; col < size; col++)
            {
                int index = col * size + row;

                TileDefinition def = map.layout[index];

                tiles.Add(new TileLayout
                {
                    tileType = def.tileType,
                    side = def.side
                });
            }

            rows.Add(new RowLayout { tiles = tiles });
        }

        return new BoardLayout { rows = rows };
    }

}
