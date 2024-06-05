using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PassiveAbilityType
{
    ADRENALIN = 1,
    EXPLOSION = 2,
    BLADE_DANCE = 3,
    INFLUENCE_AURA = 4,
    PROTECT = 5,
    ALERT = 6,
    CONTROL_SHIP = 7
}

public interface IPassiveAbility
{
    PassiveAbilityType AbilityType { get; }

    void Apply();
    bool IsDisabled();
}