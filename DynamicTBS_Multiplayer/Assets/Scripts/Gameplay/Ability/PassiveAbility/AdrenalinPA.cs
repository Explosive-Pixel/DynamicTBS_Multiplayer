using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdrenalinPA : MonoBehaviour, IPassiveAbility
{
    private Character owner;

    private void Awake()
    {
        owner = gameObject.GetComponent<Character>();
    }

    public void Apply()
    {
        CharacterEvents.OnCharacterTakesDamage += ResetActiveAbilityCooldown;
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