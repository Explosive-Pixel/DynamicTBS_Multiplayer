using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicChar : Character
{
    public MechanicChar(Player side) : base(side)
    {
        this.maxHitPoints = 2;
        this.moveSpeed = 1;
        this.attackRange = 1;

        this.activeAbility = new ChangeFloorAA(this);
        this.passiveAbility = new SteadyStandPA();

        this.characterSprite = side.GetPlayerType() == PlayerType.blue ? SpriteManager.BLUE_MECHANIC_SPRITE : SpriteManager.PINK_MECHANIC_SPRITE;

        Init();
    }
}