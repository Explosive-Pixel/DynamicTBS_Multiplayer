using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicChar : Character
{
    public MechanicChar(Player side, GameObject characterGameObject) : base(side, characterGameObject)
    {
        this.maxHitPoints = 2;
        this.moveSpeed = 1;
        this.attackRange = 1;

        this.activeAbility = new ChangeFloorAA();
        this.passiveAbility = new SteadyStandPA();
    }
}