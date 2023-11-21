using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptainChar : Character
{
    private void Start()
    {
        characterType = CharacterType.CaptainChar;
    }

    public override void Die()
    {
        base.Die();
        GameplayEvents.GameIsOver(PlayerManager.GetOtherSide(Side), GameOverCondition.MASTER_DIED);
    }
}