using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankChar : Character
{
    public static readonly string name = "Tank";

    public TankChar(Player side) : base(side)
    {
        this.characterType = CharacterType.TankChar;
        this.maxHitPoints = 4;
        this.moveSpeed = 1;
        this.attackRange = 1;

        this.activeAbility = new BlockAA(this);
        this.passiveAbility = new PullDamagePA(this);

        Init();
    }

    protected override Sprite CharacterSprite(Player side)
    {
        return side.GetPlayerType() == PlayerType.blue ? SpriteManager.BLUE_TANK_SPRITE : SpriteManager.PINK_TANK_SPRITE;
    }

    protected override GameObject CharacterPrefab(Player side)
    {
        return side.GetPlayerType() == PlayerType.blue ? PrefabManager.BLUE_TANK_PREFAB : PrefabManager.PINK_TANK_PREFAB;
    }
}