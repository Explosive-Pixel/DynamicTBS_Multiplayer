using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActiveAbilityType
{
    BLOCK,
    CHANGE_FLOOR,
    ELECTRIFY,
    HEAL,
    JUMP,
    POWERSHOT,
    TAKE_CONTROL
}

public interface IActiveAbility
{
    ActiveAbilityType AbilityType { get; }
    int Cooldown { get; }
    void Execute();

    int CountActionDestinations();

    void ShowActionPattern();
}