using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PullDamagePA : IPassiveAbility
{
    private static PatternType pullDamagePatternType = PatternType.Cross;

    private Character owner;

    public PullDamagePA(Character character)
    {
        owner = character;
    }

    public void Apply() 
    {
        List<Character> characters = CharacterHandler.GetAllLivingCharacters();

        foreach (Character character in characters)
        {
            var defaultNetDamage = character.netDamage;
            character.netDamage = (damage) =>
            {
                if (character.GetPassiveAbility().GetType() == typeof(PullDamagePA))
                {
                    return defaultNetDamage(damage);
                }

                if (owner.isDamageable(damage) && CharacterHandler.AlliedNeighbors(character, owner, pullDamagePatternType))
                {
                    int netDamage = damage - owner.hitPoints;
                    if(netDamage < defaultNetDamage(damage))
                    {
                        return netDamage;
                    }
                }

                return defaultNetDamage(damage);
            };
        }

        CharacterEvents.OnCharacterReceivesDamage += AbsorbDamage;
    }

    private void AbsorbDamage(Character character, int damage)
    {
        if (owner.IsDead())
        {
            CharacterEvents.OnCharacterReceivesDamage -= AbsorbDamage;
            return;
        }

        if (character.GetPassiveAbility().GetType() == typeof(PullDamagePA))
        {
            return;
        }

        if(owner.isDamageable(damage) && CharacterHandler.AlliedNeighbors(character, owner, pullDamagePatternType))
        {
            owner.TakeDamage(damage);
        }
    }


    ~PullDamagePA()
    {
        CharacterEvents.OnCharacterReceivesDamage -= AbsorbDamage;
    }
}