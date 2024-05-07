using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlacementEvents
{
    public delegate void PlaceCharacter(Character character);
    public static event PlaceCharacter OnPlaceCharacter;

    public static void CharacterPlaced(Character character)
    {
        if (OnPlaceCharacter != null)
            OnPlaceCharacter(character);
    }
}