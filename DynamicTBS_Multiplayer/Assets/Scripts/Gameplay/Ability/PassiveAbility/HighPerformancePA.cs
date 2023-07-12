using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HighPerformancePA : MonoBehaviour, IPassiveAbility
{
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
}