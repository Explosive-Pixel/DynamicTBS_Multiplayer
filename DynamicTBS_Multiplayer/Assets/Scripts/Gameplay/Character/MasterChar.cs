using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterChar : Character
{
    private void Start()
    {
        characterType = CharacterType.MasterChar;
    }

    public override void Die()
    {
        base.Die();
        GameplayEvents.GameIsOver(PlayerManager.GetOtherSide(Side), GameOverCondition.MASTER_DIED);
    }
}