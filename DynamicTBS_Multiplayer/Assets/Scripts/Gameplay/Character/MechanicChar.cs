using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicChar : Character
{
    public static readonly string name = "Mechanic";

    public MechanicChar(Player side) : base(side)
    {

        this.characterPrefab = GameObject.Instantiate(side.GetPlayerType() == PlayerType.blue ? PrefabManager.BLUE_MECHANIC_PREFAB : PrefabManager.PINK_MECHANIC_PREFAB);
        this.characterType = CharacterType.MechanicChar;
        this.maxHitPoints = 2;
        this.moveSpeed = 1;
        this.attackRange = 1;

        this.activeAbility = new ChangeFloorAA(this);
        this.passiveAbility = new SteadyStandPA(this);

        Init();
    }

    protected override Sprite CharacterSprite(Player side)
    {
        return side.GetPlayerType() == PlayerType.blue ? SpriteManager.BLUE_MECHANIC_SPRITE : SpriteManager.PINK_MECHANIC_SPRITE;
    }
}