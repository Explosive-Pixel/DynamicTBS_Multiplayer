using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HighPerformancePA : MonoBehaviour, IPassiveAbility
{
    private CharacterMB owner;

    private void Awake()
    {
        owner = gameObject.GetComponent<CharacterMB>();
    }

    public void Apply()
    {
        owner.AttackDamage *= 2;
        owner.MovePattern = PatternType.Star;
    }
}