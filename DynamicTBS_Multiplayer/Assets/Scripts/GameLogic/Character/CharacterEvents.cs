using UnityEngine;

public static class CharacterEvents
{
    public delegate void CharacterDeath(Character character, Vector3 lastPosition);
    public static event CharacterDeath OnCharacterDeath;

    public delegate void CharacterDamage(Character character, int damage);
    public static event CharacterDamage OnCharacterReceivesDamage;
    public static event CharacterDamage OnCharacterTakesDamage;

    public static void CharacterDies(Character character, Vector3 lastPosition)
    {
        if (OnCharacterDeath != null)
            OnCharacterDeath(character, lastPosition);
    }

    public static void CharacterReceivesDamage(Character character, int damage)
    {
        if (OnCharacterReceivesDamage != null)
            OnCharacterReceivesDamage(character, damage);
    }

    public static void CharacterTakesDamage(Character character, int damage)
    {
        if (OnCharacterTakesDamage != null)
            OnCharacterTakesDamage(character, damage);
    }
}