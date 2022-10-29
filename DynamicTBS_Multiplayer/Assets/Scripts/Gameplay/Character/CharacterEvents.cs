using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterEvents
{
    public delegate void CharacterDeath(Vector3 position);
    public static event CharacterDeath OnCharacterDeath;

    public static void KillCharacter(Vector3 position)
    {
        if (OnCharacterDeath != null)
            OnCharacterDeath(position);
    }
}