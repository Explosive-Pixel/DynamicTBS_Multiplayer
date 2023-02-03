using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerChar : Character
{
    public static readonly string name = "Runner";

    public RunnerChar(Player side) : base(side)
    {
        this.characterType = CharacterType.RunnerChar;
        this.maxHitPoints = 1;
        this.moveSpeed = 2;
        this.attackRange = 1;

        this.activeAbility = new JumpAA(this);
        this.passiveAbility = new HighPerformancePA(this);

        Init();
    }

    protected override Sprite CharacterSprite(Player side)
    {
        return side.GetPlayerType() == PlayerType.blue ? SpriteManager.BLUE_RUNNER_SPRITE : SpriteManager.PINK_RUNNER_SPRITE;
    }

    protected override GameObject CharacterPrefab(Player side)
    {
        return side.GetPlayerType() == PlayerType.blue ? PrefabManager.BLUE_RUNNER_PREFAB : PrefabManager.PINK_RUNNER_PREFAB;
    }
}