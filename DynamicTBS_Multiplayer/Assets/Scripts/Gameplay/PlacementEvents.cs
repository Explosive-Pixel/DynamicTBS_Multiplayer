using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlacementEvents
{
    public delegate void PlacementStart();

    public static event PlacementStart OnPlacementStart;

    public static void StartPlacement()
    {
        if (OnPlacementStart != null)
            OnPlacementStart();
    }
}