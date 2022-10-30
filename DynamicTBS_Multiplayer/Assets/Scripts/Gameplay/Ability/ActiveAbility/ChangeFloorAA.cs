using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFloorAA : IActiveAbility
{
    public static int radius = 2;
    public static int radiusWithInhabitants = 1;

    public int Cooldown { get { return 3; } }
    private ChangeFloorAAHandler changeFloorAAHandler;

    Character character;

    public ChangeFloorAA(Character character)
    {
        this.character = character;
        changeFloorAAHandler = GameObject.Find("ActiveAbilityObject").GetComponent<ChangeFloorAAHandler>();
    }

    public void Execute()
    {
        changeFloorAAHandler.ExecuteChangeFloorAA(character);
    }
}
