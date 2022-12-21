using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterChar : Character
{
    public MasterChar(Player side) : base(side)
    {
        this.characterPrefab = GameObject.Instantiate(side.GetPlayerType() == PlayerType.blue ? PrefabManager.BLUE_MASTER_PREFAB : PrefabManager.PINK_MASTER_PREFAB);
        this.characterType = CharacterType.MasterChar;
        this.maxHitPoints = 3;
        this.moveSpeed = 1;
        this.attackRange = 0;

        this.activeAbility = new TakeControlAA(this);
        this.passiveAbility = new InfluenceAuraPA(this);

        
        Init();
    }

    protected override Sprite CharacterSprite(Player side)
    {
        return side.GetPlayerType() == PlayerType.blue ? SpriteManager.BLUE_MASTER_SPRITE : SpriteManager.PINK_MASTER_SPRITE;
    }


    public override void Die()
    {
        base.Die();
        GameplayEvents.GameIsOver(side.GetPlayerType() == PlayerType.blue ? PlayerType.blue : PlayerType.pink);
    }
}