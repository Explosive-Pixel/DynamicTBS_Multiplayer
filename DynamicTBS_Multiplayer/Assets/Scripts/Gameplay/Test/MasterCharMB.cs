using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterCharMB : CharacterMB
{

    private void Start()
    {
        characterType = CharacterType.RunnerChar;
    }

    public override void Die()
    {
        base.Die();
        GameplayEvents.GameIsOver(PlayerManager.GetOtherSide(Side), GameOverCondition.MASTER_DIED);
    }
}