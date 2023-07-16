using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PassiveAbilityType
{
    ADRENALIN,
    EXPLODE,
    HIGH_PERFORMANCE,
    INFLUENCE_AURA,
    PULL_DAMAGE,
    STEADY_STAND
}

public interface IPassiveAbility
{
    PassiveAbilityType AbilityType { get; }

    void Apply();
    bool IsDisabled();
}