using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdrenalinPA : MonoBehaviour, IPassiveAbility
{
    public PassiveAbilityType AbilityType { get { return PassiveAbilityType.ADRENALIN; } }

    private Character owner;

    private void Awake()
    {
        owner = gameObject.GetComponent<Character>();
    }

    public void Apply()
    {
        CharacterEvents.OnCharacterTakesDamage += ResetActiveAbilityCooldown;
    }

    public bool IsDisabled()
    {
        return false;
    }

    private void ResetActiveAbilityCooldown(Character character, int damage)
    {
        if (character == owner)
        {
            AudioEvents.AdrenalinRelease();
            owner.ActiveAbilityCooldown = 0;
        }
    }

    private void OnDestroy()
    {
        CharacterEvents.OnCharacterTakesDamage -= ResetActiveAbilityCooldown;
    }
}