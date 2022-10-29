using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlacementEvents
{
    public delegate void CharacterPlacement(Character character);
    public static event CharacterPlacement OnCharacterSelectionForPlacement;

    public delegate void PlacementOrder();
    public static event PlacementOrder OnAdvancePlacementOrder;

    public delegate void PlacementMessageText();
    public static event PlacementMessageText OnPlacementMessageChange;

    public static void SelectCharacterForPlacement(Character character)
    {
        if (OnCharacterSelectionForPlacement != null)
            OnCharacterSelectionForPlacement(character);
    }

    public static void AdvancePlacementOrder()
    {
        if (OnAdvancePlacementOrder != null)
            OnAdvancePlacementOrder();
    }

    public static void ChangePlacementMessage()
    {
        if (OnPlacementMessageChange != null)
            OnPlacementMessageChange();
    }
}