using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicChar : Character
{
    public static readonly string name = "Medic";

    public MedicChar(Player side) : base(side)
    {
        this.characterPrefab = GameObject.Instantiate(side.GetPlayerType() == PlayerType.blue ? PrefabManager.BLUE_MEDIC_PREFAB : PrefabManager.PINK_MEDIC_PREFAB);
        this.characterType = CharacterType.MedicChar;
        this.maxHitPoints = 2;
        this.moveSpeed = 1;
        this.attackRange = 1;

        this.activeAbility = new HealAA(this);
        this.passiveAbility = new AdrenalinPA(this);

        Init();
    }

    protected override Sprite CharacterSprite(Player side)
    {
        return side.GetPlayerType() == PlayerType.blue ? SpriteManager.BLUE_MEDIC_SPRITE : SpriteManager.PINK_MEDIC_SPRITE;
    }
}