using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAA : IActiveAbility
{
    public static PatternType movePattern = PatternType.Star;
    public static int distance = 3;

    public int Cooldown { get { return 2; } }

    private JumpAAAction jumpAAAction;

    Character character;

    public JumpAA(Character character)
    {
        this.character = character;
        jumpAAAction = GameObject.Find("ActionRegistry").GetComponent<JumpAAAction>();
    }

    public void Execute()
    {
        jumpAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(jumpAAAction);
    }
}