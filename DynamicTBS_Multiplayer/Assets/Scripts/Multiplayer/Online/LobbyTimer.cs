using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyTimer
{
    #region Helper classes
    class PlayerTime
    {
        public float timeLeft;
        public int debuff = 0;
    }

    #endregion

    private float draftAndPlacementTime;
    private float gameplayTime;

    private PlayerType currentPlayer;
    private Dictionary<PlayerType, PlayerTime> timePerPlayer = new Dictionary<PlayerType, PlayerTime>();

    private TimerType currentTimerType = TimerType.DRAFT_AND_PLACEMENT;

    public LobbyTimer(float draftAndPlacementTime, float gameplayTime)
    {
        this.draftAndPlacementTime = draftAndPlacementTime;
        this.gameplayTime = gameplayTime;

        timePerPlayer.Add(PlayerType.pink, new PlayerTime { timeLeft = draftAndPlacementTime });
        timePerPlayer.Add(PlayerType.blue, new PlayerTime { timeLeft = draftAndPlacementTime });
    }

    public void UpdateGameInfo(PlayerType currentPlayer, GamePhase gamePhase)
    {
        this.currentPlayer = currentPlayer;
        this.currentTimerType = gamePhase == GamePhase.GAMEPLAY ? TimerType.GAMEPLAY : TimerType.DRAFT_AND_PLACEMENT;
    }

    public void UpdateTime()
    {
        if(timePerPlayer[currentPlayer].timeLeft > 0)
        {
            timePerPlayer[currentPlayer].timeLeft--;
            return;
        }
        

        if(currentTimerType == TimerType.GAMEPLAY)
        {
            timePerPlayer[currentPlayer].debuff++;
        }
    }
}
