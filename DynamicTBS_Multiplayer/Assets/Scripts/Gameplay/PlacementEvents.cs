using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlacementEvents
{
    public delegate void PlacementStart();
    public static event PlacementStart OnPlacementStart;

    public delegate void CharacterPlacement(Character character);
    public static event CharacterPlacement OnCharacterSelection;

    public static void StartPlacement()
    {
        if (OnPlacementStart != null)
            OnPlacementStart();
    }

    public static void SelectCharacterForPlacement(Character character)
    {
        if (OnCharacterSelection != null)
            OnCharacterSelection(character);
    }
}