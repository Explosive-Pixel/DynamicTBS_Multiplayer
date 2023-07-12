using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlacementEvents
{
    public delegate void PlaceCharacter(CharacterMB character);
    public static event PlaceCharacter OnPlaceCharacter;

    public static void CharacterPlaced(CharacterMB character)
    {
        if (OnPlaceCharacter != null)
            OnPlaceCharacter(character);
    }
}