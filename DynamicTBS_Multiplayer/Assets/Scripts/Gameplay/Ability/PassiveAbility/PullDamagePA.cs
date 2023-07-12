using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PullDamagePA : MonoBehaviour, IPassiveAbility
{
    [SerializeField] private PatternType pullDamagePatternType; // = PatternType.Cross;

    private Character owner;

    private void Awake()
    {
        owner = gameObject.GetComponent<Character>();
    }

    public void Apply()
    {
        List<Character> characters = CharacterManager.GetAllLivingCharacters();

        foreach (Character character in characters)
        {
            var defaultNetDamage = character.netDamage;
            character.netDamage = (damage) =>
            {
                if (character.PassiveAbility.GetType() == typeof(PullDamagePA))
                {
                    return defaultNetDamage(damage);
                }

                if (!owner.isDisabled() && owner.isDamageable(damage) && CharacterManager.AlliedNeighbors(character, owner, pullDamagePatternType))
                {
                    int netDamage = damage - owner.HitPoints;
                    if (netDamage < defaultNetDamage(damage))
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
        if (character.PassiveAbility.GetType() == typeof(PullDamagePA))
        {
            return;
        }

        if (owner.isDamageable(damage) && CharacterManager.AlliedNeighbors(character, owner, pullDamagePatternType))
        {
            owner.TakeDamage(damage);
        }
    }


    private void OnDestroy()
    {
        CharacterEvents.OnCharacterReceivesDamage -= AbsorbDamage;
    }
}