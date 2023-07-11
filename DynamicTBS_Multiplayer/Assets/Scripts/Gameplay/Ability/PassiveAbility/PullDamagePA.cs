using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PullDamagePA : MonoBehaviour, IPassiveAbility
{
    [SerializeField] private PatternType pullDamagePatternType; // = PatternType.Cross;

    private CharacterMB owner;

    private void Awake()
    {
        owner = gameObject.GetComponent<CharacterMB>();
    }

    public void Apply()
    {
        List<CharacterMB> characters = CharacterManager.GetAllLivingCharacters();

        foreach (CharacterMB character in characters)
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

    private void AbsorbDamage(CharacterMB character, int damage)
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