using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterChar : Character
{
    public ShooterChar(Player side) : base(side)
    {
        this.maxHitPoints = 2;
        this.moveSpeed = 1;
        this.attackRange = 2;

        this.activeAbility = new PowershotAA();
        this.passiveAbility = new ExplodePA();
    }
}