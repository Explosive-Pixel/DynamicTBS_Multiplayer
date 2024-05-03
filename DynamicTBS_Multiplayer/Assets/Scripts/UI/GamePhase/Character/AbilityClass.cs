using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    ACTIVE,
    PASSIVE
}

public class AbilityClass : MonoBehaviour
{
    public AbilityType abilityType;
    public ActiveAbilityType activeAbilityType;
    public PassiveAbilityType passiveAbilityType;
    public PlayerType side;
    public bool disabled;
}
