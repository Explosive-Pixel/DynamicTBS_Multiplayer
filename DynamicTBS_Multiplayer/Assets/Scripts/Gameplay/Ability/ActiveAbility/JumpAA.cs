using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAA : IActiveAbility
{
    public static PatternType movePattern = PatternType.Star;
    public static int distance = 3;

    public int Cooldown { get { return 2; } }

    private JumpAAHandler jumpAAHandler;

    Character character;

    public JumpAA(Character character)
    {
        this.character = character;
        jumpAAHandler = GameObject.Find("ActiveAbilityObject").GetComponent<JumpAAHandler>();
    }

    public void Execute()
    {
        jumpAAHandler.ExecuteJumpAA(character);
    }
}