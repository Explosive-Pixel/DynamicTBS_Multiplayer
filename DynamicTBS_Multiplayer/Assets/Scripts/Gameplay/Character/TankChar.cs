using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankChar : Character
{
    public TankChar(Player side, GameObject characterGameObject) : base(side, characterGameObject)
    {
        this.maxHitPoints = 4;
        this.moveSpeed = 1;
        this.attackRange = 1;

        this.activeAbility = new BlockAA();
        this.passiveAbility = new PullDamagePA();
    }
}