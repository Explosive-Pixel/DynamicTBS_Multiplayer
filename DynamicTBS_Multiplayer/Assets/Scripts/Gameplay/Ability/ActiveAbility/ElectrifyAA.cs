using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrifyAA : IActiveAbility
{
    public static int radius = 1;

    public int Cooldown { get { return 3; } }

    Character character;
    private ElectrifyAAAction electrifyAAAction;

    public ElectrifyAA(Character character)
    {
        this.character = character;
        electrifyAAAction = GameObject.Find("ActionRegistry").GetComponent<ElectrifyAAAction>();
    }

    public void Execute()
    {
        electrifyAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(electrifyAAAction);
    }

    public int CountActionDestinations()
    {
        return electrifyAAAction.CountActionDestinations(character);
    }

    public void ShowActionPattern()
    {
        electrifyAAAction.ShowActionPattern(character);
    }
}
