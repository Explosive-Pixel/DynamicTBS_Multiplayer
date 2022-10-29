using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterEvents
{
    public delegate void CharacterDeath(Character character, Vector3 lastPosition);
    public static event CharacterDeath OnCharacterDeath;

    public static void CharacterDies(Character character, Vector3 lastPosition)
    {
        if (OnCharacterDeath != null)
            OnCharacterDeath(character, lastPosition);
    }
}