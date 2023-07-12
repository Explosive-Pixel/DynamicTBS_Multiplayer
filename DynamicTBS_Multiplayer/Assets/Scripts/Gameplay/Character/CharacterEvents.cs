using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterEvents
{
    public delegate void CharacterDeath(CharacterMB character, Vector3 lastPosition);
    public static event CharacterDeath OnCharacterDeath;

    public delegate void CharacterDamage(CharacterMB character, int damage);
    public static event CharacterDamage OnCharacterReceivesDamage;
    public static event CharacterDamage OnCharacterTakesDamage;

    public static void CharacterDies(CharacterMB character, Vector3 lastPosition)
    {
        if (OnCharacterDeath != null)
            OnCharacterDeath(character, lastPosition);
    }

    public static void CharacterReceivesDamage(CharacterMB character, int damage)
    {
        if (OnCharacterReceivesDamage != null)
            OnCharacterReceivesDamage(character, damage);
    }

    public static void CharacterTakesDamage(CharacterMB character, int damage)
    {
        if (OnCharacterTakesDamage != null)
            OnCharacterTakesDamage(character, damage);
    }
}