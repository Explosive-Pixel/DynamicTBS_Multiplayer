using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAA : IActiveAbility
{
    public static int healingRange = 2;
    public static int healingPoints = 1;

    public int Cooldown { get { return 3; } }

    private HealAAHandler healAAHandler;

    Character character;

    public HealAA(Character character)
    {
        this.character = character;
        healAAHandler = GameObject.Find("ActiveAbilityObject").GetComponent<HealAAHandler>();
    }

    public void Execute() 
    {
        healAAHandler.ExecuteHealAA(character);
    }


}