using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAA : IActiveAbility
{
    public static int healingRange = 2;
    public static int healingPoints = 1;

    public int Cooldown { get { return 3; } }

    private HealAAAction healAAAction;

    Character character;

    public HealAA(Character character)
    {
        this.character = character;
        healAAAction = GameObject.Find("ActionRegistry").GetComponent<HealAAAction>();
    }

    public void Execute() 
    {
        healAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(healAAAction);
    }


}