using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicChar : Character
{
    public MedicChar(Player side) : base(side)
    {
        this.maxHitPoints = 2;
        this.moveSpeed = 1;
        this.attackRange = 1;

        this.activeAbility = new HealAA(this);
        this.passiveAbility = new AdrenalinPA();

        this.characterSprite = side.GetPlayerType() == PlayerType.blue ? SpriteManager.BLUE_MEDIC_SPRITE : SpriteManager.PINK_MEDIC_SPRITE;

        Init();
    }
}