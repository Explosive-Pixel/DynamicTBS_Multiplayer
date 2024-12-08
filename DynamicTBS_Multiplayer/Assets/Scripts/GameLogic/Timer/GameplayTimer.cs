using System;
using UnityEngine;

public class GameplayTimer : GameTimer
{
    private readonly float debuffRate = 0.25f;
    private readonly int maxDebuffs = 3;

    private int debuff = 0;

    public override void SetActive(DateTime startTime)
    {
        StartTimer(startTime, originalDuration * Mathf.Pow(1 - debuffRate, debuff));
    }

    public override void SetInactive()
    {
        StopTimer();
    }

    public override void DrawNoTimeLeftConsequences()
    {
        debuff++;

        if (debuff == maxDebuffs)
        {
            GameplayEvents.GameIsOver(PlayerManager.GetOtherSide(playerType), GameOverCondition.PLAYER_TIMEOUT);
        }

        GameplayEvents.AbortCurrentPlayerTurn(playerType, GameplayManager.GetRemainingActions(), AbortTurnCondition.PLAYER_TIMEOUT);
    }
}
