using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowershotAA : IActiveAbility
{
    public static int powershotDamage = 1;
    public static int selfDamage = 1;
    public int Cooldown { get { return 3; } }

    private PowershotAAAction powershotAAAction;

    Character character;

    public PowershotAA(Character character)
    {
        this.character = character;
        powershotAAAction = GameObject.Find("ActionRegistry").GetComponent<PowershotAAAction>();
    }
    public void Execute()
    {
        powershotAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(powershotAAAction);
    }

    public int CountActionDestinations()
    { 
        return powershotAAAction.CountActionDestinations(character);
    }

    public void ShowActionPattern()
    {
        powershotAAAction.ShowActionPattern(character);
    }
}