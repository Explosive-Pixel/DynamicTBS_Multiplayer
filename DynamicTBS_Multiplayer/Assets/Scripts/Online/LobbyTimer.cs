using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyTimer
{
    private const float timerUpdateInterval = 1f;

    #region Helper classes
    class PlayerTime
    {
        public float timeLeft;
        public int debuff = 0;
    }

    #endregion

    private TimerSetupType timerSetup;
    public TimerSetupType TimerSetup { get { return timerSetup; } }

    private float DraftAndPlacementTime { get { return Timer.GetTimeSetup(timerSetup)[TimerType.DRAFT_AND_PLACEMENT]; } }
    private float GameplayTime { get { return Timer.GetTimeSetup(timerSetup)[TimerType.GAMEPLAY]; } }

    private PlayerType currentPlayer = PlayerManager.StartPlayer[GamePhase.DRAFT];
    private Dictionary<PlayerType, PlayerTime> timePerPlayer = new Dictionary<PlayerType, PlayerTime>();

    private TimerType currentTimerType = TimerType.DRAFT_AND_PLACEMENT;
    private GamePhase currentGamePhase = GamePhase.DRAFT;

    public GamePhase CurrentGamePhase { get { return currentGamePhase; } }

    public LobbyTimer(TimerSetupType timerSetup)
    {
        this.timerSetup = timerSetup;

        timePerPlayer.Add(PlayerType.pink, new PlayerTime { timeLeft = DraftAndPlacementTime });
        timePerPlayer.Add(PlayerType.blue, new PlayerTime { timeLeft = DraftAndPlacementTime });
    }

    public void UpdateGameInfo(PlayerType currentPlayer, GamePhase gamePhase)
    {
        this.currentPlayer = currentPlayer;
        this.currentTimerType = gamePhase == GamePhase.GAMEPLAY ? TimerType.GAMEPLAY : TimerType.DRAFT_AND_PLACEMENT;

        if (currentGamePhase != gamePhase)
        {
            this.currentGamePhase = gamePhase;
            float newTime = currentTimerType == TimerType.GAMEPLAY ? GameplayTime : DraftAndPlacementTime;

            timePerPlayer[PlayerType.pink].timeLeft = newTime;
            timePerPlayer[PlayerType.blue].timeLeft = newTime;
        }

        if(gamePhase == GamePhase.GAMEPLAY)
        {
            timePerPlayer[currentPlayer].timeLeft = GameplayTime * Mathf.Pow(1 - Timer.debuffRate, timePerPlayer[currentPlayer].debuff);
        }
    }

    private float delta = 0;
    public void UpdateTime(int lobbyId)
    {
        if(timePerPlayer[currentPlayer].timeLeft > 0)
        {
            timePerPlayer[currentPlayer].timeLeft -= Time.deltaTime;
        }

        delta += Time.deltaTime;
        if (delta >= timerUpdateInterval)
        {
            BroadcastTimerInfo(lobbyId);
            delta = 0f;
        }
    }

    private void BroadcastTimerInfo(int lobbyId)
    {
        if (timePerPlayer[currentPlayer].timeLeft > 0)
        {
            OnlineServer.Instance.Broadcast(new MsgSyncTimer
            {
                pinkTimeLeft = timePerPlayer[PlayerType.pink].timeLeft,
                blueTimeLeft = timePerPlayer[PlayerType.blue].timeLeft,
                pinkDebuff = timePerPlayer[PlayerType.pink].debuff,
                blueDebuff = timePerPlayer[PlayerType.blue].debuff,
            }, lobbyId);
        }
        else
        {
            if (currentTimerType == TimerType.GAMEPLAY)
            {
                timePerPlayer[currentPlayer].debuff++;
            }

            OnlineServer.Instance.Broadcast(new MsgServerNotification
            {
                serverNotification = ServerNotification.TIMEOUT,
                gamePhase = currentGamePhase,
                currentPlayer = currentPlayer,
                currentPlayerTimerDebuff = timePerPlayer[currentPlayer].debuff
            }, lobbyId);
        }
    }
}
