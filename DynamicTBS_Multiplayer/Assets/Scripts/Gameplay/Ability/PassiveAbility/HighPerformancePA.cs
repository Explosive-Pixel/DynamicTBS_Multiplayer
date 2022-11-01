using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HighPerformancePA : IPassiveAbility
{
    private Character owner;

    public HighPerformancePA(Character character)
    {
        owner = character;
    }

    public void Apply() 
    {
        owner.attackDamage *= 2;
        owner.movePattern = PatternType.Star;
    }
}