using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAA : IActiveAbility
{
    public static int healingRange = 2;
    public static int healingPoints = 1;

    private HealAAHandler healAAHandler;

    Character character;

    public HealAA(Character character)
    {
        this.character = character;
        healAAHandler = GameObject.Find("ActiveAbilityHandler").GetComponent<HealAAHandler>();
    }

    public void Execute() 
    {
        healAAHandler.ExecuteHealAA(character);
    }


}