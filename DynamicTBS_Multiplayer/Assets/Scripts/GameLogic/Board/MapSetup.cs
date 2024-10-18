using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSetup
{
    public MapType MapType { get; private set; }

    public string MapCategory { get { return (MapType == MapType.EXPLOSIVE || MapType == MapType.LABYRINTH) ? "COMPETITIVE" : "CASUAL"; } }

    public MapSetup(MapType mapType)
    {
        MapType = mapType;
    }
}
