using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterChar : Character
{
    public MasterChar(Player side) : base(side)
    {
        this.maxHitPoints = 3;
        this.moveSpeed = 1;
        this.attackRange = 0;

        this.activeAbility = new TakeControlAA(this);
        this.passiveAbility = new InfluenceAuraPA();

        this.characterSprite = side.GetPlayerType() == PlayerType.blue ? SpriteManager.BLUE_MASTER_SPRITE : SpriteManager.PINK_MASTER_SPRITE;

        Init();
    }

    public override void Die()
    {
        base.Die();
        CharacterHandler characterHandler = GameObject.Find("GameplayCanvas").GetComponent<CharacterHandler>();
        characterHandler.InformAboutGameOver(side.GetPlayerType() == PlayerType.blue ? PlayerType.blue : PlayerType.pink);
    }
}