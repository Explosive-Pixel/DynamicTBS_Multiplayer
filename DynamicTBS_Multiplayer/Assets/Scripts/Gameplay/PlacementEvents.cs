using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlacementEvents
{
    public delegate void PlacementOrder();
    public static event PlacementOrder OnPlacementStart;

    public delegate void PlacementMessageText();
    public static event PlacementMessageText OnPlacementMessageChange;

    public delegate void PlaceCharacter(Character character);
    public static event PlaceCharacter OnPlaceCharacter;

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

    public static void CharacterPlaced(Character character)
    {
        if (OnPlaceCharacter != null)
            OnPlaceCharacter(character);
    }
}