using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterChar : Character
{
    public ShooterChar(Player side) : base(side)
    {
        this.characterPrefab = GameObject.Instantiate(side.GetPlayerType() == PlayerType.blue ? PrefabManager.BLUE_SHOOTER_PREFAB : PrefabManager.PINK_SHOOTER_PREFAB);
        this.characterType = CharacterType.ShooterChar;
        this.maxHitPoints = 2;
        this.moveSpeed = 1;
        this.attackRange = 2;

        this.activeAbility = new PowershotAA(this);
        this.passiveAbility = new ExplodePA(this);

        Init();
    }

    protected override Sprite CharacterSprite(Player side)
    {
        return side.GetPlayerType() == PlayerType.blue ? SpriteManager.BLUE_SHOOTER_SPRITE : SpriteManager.PINK_SHOOTER_SPRITE;
    }
}