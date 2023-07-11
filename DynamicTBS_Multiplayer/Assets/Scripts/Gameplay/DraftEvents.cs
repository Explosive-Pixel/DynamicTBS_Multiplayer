using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DraftEvents
{
    public delegate void CharacterCreation(CharacterMB character);
    public static event CharacterCreation OnCharacterCreated;

    public static void CharacterCreated(CharacterMB character)
    {
        if (OnCharacterCreated != null)
            OnCharacterCreated(character);
    }
}