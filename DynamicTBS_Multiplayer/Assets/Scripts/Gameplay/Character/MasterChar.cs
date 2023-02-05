using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterChar : Character
{
    public static readonly string name = "Captain";

    public MasterChar(Player side) : base(side)
    {
        this.characterType = CharacterType.MasterChar;
        this.maxHitPoints = 3;
        this.moveSpeed = 1;
        this.attackRange = 0;

        this.activeAbility = new TakeControlAA(this);
        this.passiveAbility = new InfluenceAuraPA(this);
        
        Init();
    }

    protected override GameObject CharacterPrefab(Player side)
    {
        return side.GetPlayerType() == PlayerType.blue ? PrefabManager.BLUE_MASTER_PREFAB : PrefabManager.PINK_MASTER_PREFAB;
    }


    public override void Die()
    {
        base.Die();
        GameplayEvents.GameIsOver(PlayerManager.GetOtherPlayer(side).GetPlayerType(), GameOverCondition.MASTER_DIED);
    }
}