using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActiveAbility
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
    int Cooldown { get; }
    void Execute();

    int CountActionDestinations();

    void ShowActionPattern();
}