using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFloorAA : IActiveAbility
{
    public static int radius = 2;
    public static int radiusWithInhabitants = 1;

    public int Cooldown { get { return 3; } }
    private ChangeFloorAAAction changeFloorAAAction;

    Character character;

    public ChangeFloorAA(Character character)
    {
        this.character = character;
        changeFloorAAAction = GameObject.Find("ActionRegistry").GetComponent<ChangeFloorAAAction>();
    }

    public void Execute()
    {
        changeFloorAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(changeFloorAAAction);
    }

    public int CountActionDestinations()
    {
        return changeFloorAAAction.CountActionDestinations(character);
    }
}
