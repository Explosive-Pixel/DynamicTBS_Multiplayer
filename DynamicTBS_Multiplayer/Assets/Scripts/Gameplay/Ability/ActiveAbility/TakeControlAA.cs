using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeControlAA : IActiveAbility
{
    private TakeControlAAHandler takeControlAAHandler;

    Character character;

    public TakeControlAA(Character character)
    {
        this.character = character;
        takeControlAAHandler = GameObject.Find("ActiveAbilityObject").GetComponent<TakeControlAAHandler>();
    }

    public void Execute() 
    {
        takeControlAAHandler.ExecuteTakeControlAA(character);
    }
}