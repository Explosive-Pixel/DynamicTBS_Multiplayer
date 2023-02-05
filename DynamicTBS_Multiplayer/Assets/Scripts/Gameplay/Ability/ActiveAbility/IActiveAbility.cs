using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActiveAbility
{
    int Cooldown { get; }
    void Execute();

    int CountActionDestinations();

    void ShowActionPattern();
}