using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BladeDancePA : MonoBehaviour, IPassiveAbility
{
    public PassiveAbilityType AbilityType { get { return PassiveAbilityType.BLADE_DANCE; } }

    private Character owner;

    private void Awake()
    {
        owner = gameObject.GetComponent<Character>();
    }

    public void Apply()
    {
        owner.AttackDamage *= 2;
        owner.MovePattern = PatternType.Star;
    }

    public bool IsDisabled()
    {
        return false;
    }
}