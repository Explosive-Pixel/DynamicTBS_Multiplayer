using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PassiveAbilityType
{
    ADRENALIN = 1,
    EXPLODE = 2,
    HIGH_PERFORMANCE = 3,
    INFLUENCE_AURA = 4,
    PULL_DAMAGE = 5,
    STEADY_STAND = 6
}

public interface IPassiveAbility
{
    PassiveAbilityType AbilityType { get; }

    void Apply();
    bool IsDisabled();
}