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
        CharacterEvents.OnCharacterReceivesDamage += ResetActiveAbilityCooldown;
    }

    public bool IsDisabled()
    {
        return false;
    }

    private void ResetActiveAbilityCooldown(Character character, int damage)
    {
        if (character == owner && !character.IsDead())
        {
            if (owner.ActiveAbilityCooldown > 0)
                AudioEvents.AdrenalinRelease();

            owner.ActiveAbilityCooldown = 0;
        }
    }

    private void OnDestroy()
    {
        CharacterEvents.OnCharacterReceivesDamage -= ResetActiveAbilityCooldown;
    }
}