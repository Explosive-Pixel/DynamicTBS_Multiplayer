using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIEvents
{
    public delegate void Selection();
    public static event Selection OnMapSelected;

    public static void MapSelected()
    {
        if (OnMapSelected != null)
            OnMapSelected();
    }
}
