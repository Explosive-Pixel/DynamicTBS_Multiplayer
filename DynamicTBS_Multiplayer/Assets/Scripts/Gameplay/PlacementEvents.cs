using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlacementEvents
{
    public delegate void PlacementOrder();
    public static event PlacementOrder OnPlacementStart;

    public delegate void PlacementMessageText();
    public static event PlacementMessageText OnPlacementMessageChange;

    public static void StartPlacement()
    {
        if (OnPlacementStart != null)
        {
            OnPlacementStart();
        }
    }

    public static void ChangePlacementMessage()
    {
        if (OnPlacementMessageChange != null)
            OnPlacementMessageChange();
    }
}