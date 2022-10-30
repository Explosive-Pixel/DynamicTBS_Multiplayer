using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowershotAA : IActiveAbility
{
    public static int powershotDamage = 1;
    public static int selfDamage = 1;
    public int Cooldown { get { return 3; } }

    private PowershotAAHandler powershotAA;

    Character character;

    public PowershotAA(Character character)
    {
        this.character = character;
        powershotAA = GameObject.Find("ActiveAbilityObject").GetComponent<PowershotAAHandler>();
    }
    public void Execute()
    {
        powershotAA.ExecutePowershotAA(character);
    }
}