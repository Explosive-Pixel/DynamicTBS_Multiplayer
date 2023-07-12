/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterChar : Character
{
    public static readonly string name = "Shooter";

    public ShooterChar(Player side) : base(side)
    {
        this.characterType = CharacterType.ShooterChar;
        this.maxHitPoints = 2;
        this.moveSpeed = 1;
        this.attackRange = 2;

        this.activeAbility = new PowershotAA(this);
        this.passiveAbility = new ExplodePA(this);

        Init();
    }

    protected override GameObject CharacterPrefab(Player side)
    {
        return side.GetPlayerType() == PlayerType.blue ? PrefabManager.BLUE_SHOOTER_PREFAB : PrefabManager.PINK_SHOOTER_PREFAB;
    }
}*/