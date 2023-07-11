using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptainCharMB : CharacterMB
{
    public override void Die()
    {
        base.Die();
        GameplayEvents.GameIsOver(PlayerManager.GetOtherSide(Side), GameOverCondition.MASTER_DIED);
    }
}
