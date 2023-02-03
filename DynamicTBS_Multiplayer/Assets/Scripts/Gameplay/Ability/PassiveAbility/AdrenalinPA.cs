using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdrenalinPA : IPassiveAbility
{
    private Character owner;

    public AdrenalinPA(Character character)
    {
        owner = character;
    }

    public void Apply() 
    {
        CharacterEvents.OnCharacterTakesDamage += ResetActiveAbilityCooldown;
    }

    private void ResetActiveAbilityCooldown(Character character, int damage)
    {
        if (owner.IsDead())
        {
            CharacterEvents.OnCharacterTakesDamage -= ResetActiveAbilityCooldown;
            return;
        }

        if (character == owner)
        {
            AudioEvents.AdrenalinRelease();
            owner.ActiveAbilityCooldown = 0;
        }
    }

    ~AdrenalinPA()
    {
        CharacterEvents.OnCharacterTakesDamage -= ResetActiveAbilityCooldown;
    }
}