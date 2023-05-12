using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class LobbyTimer
{
    #region Helper classes
    class PlayerTime
    {
        private float startTime;
        public float timeLeft;
        public int debuff = 0;

        public float StartTime { get { return startTime; } set { startTime = value; timeLeft = value; } }
    }

    #endregion

    private TimerSetupType timerSetup;
    public TimerSetupType TimerSetup { get { return timerSetup; } }

    private float DraftAndPlacementTime { get { return Timer.GetTimeSetup(timerSetup)[TimerType.DRAFT_AND_PLACEMENT]; } }
    private float GameplayTime { get { return Timer.GetTimeSetup(timerSetup)[TimerType.GAMEPLAY]; } }

    private PlayerType currentPlayer = PlayerManager.StartPlayer[GamePhase.DRAFT];
    private Dictionary<PlayerType, PlayerTime> timePerPlayer = new Dictionary<PlayerType, PlayerTime>();

    private TimerType currentTimerType = TimerType.DRAFT_AND_PLACEMENT;
    private GamePhase currentGamePhase = GamePhase.NONE;

    public GamePhase CurrentGamePhase { get { return currentGamePhase; } }

    private DateTime startTime;
    private bool timerRanOff = false;

    public LobbyTimer(TimerSetupType timerSetup)
    {
        this.timerSetup = timerSetup;

        timePerPlayer.Add(PlayerType.pink, new PlayerTime { StartTime = DraftAndPlacementTime });
        timePerPlayer.Add(PlayerType.blue, new PlayerTime { StartTime = DraftAndPlacementTime });
    }

    public void UpdateGameInfo(PlayerType currentPlayer, GamePhase gamePhase, int lobbyId)
    {
        this.currentPlayer = currentPlayer;
        timerRanOff = false;

        this.currentTimerType = gamePhase == GamePhase.GAMEPLAY ? TimerType.GAMEPLAY : TimerType.DRAFT_AND_PLACEMENT;

        if (currentGamePhase != gamePhase)
        {
            this.currentGamePhase = gamePhase;

            if (gamePhase == GamePhase.NONE)
                return;

            float newTime = currentTimerType == TimerType.GAMEPLAY ? GameplayTime : DraftAndPlacementTime;

            timePerPlayer[PlayerType.pink].StartTime = newTime;
            timePerPlayer[PlayerType.blue].StartTime = newTime;
        }

        if(gamePhase == GamePhase.GAMEPLAY)
        {
            timePerPlayer[currentPlayer].StartTime = GameplayTime * Mathf.Pow(1 - Timer.debuffRate, timePerPlayer[currentPlayer].debuff);
        } else
        {
            PlayerType lastPlayer = PlayerManager.GetOtherSide(currentPlayer);
            timePerPlayer[lastPlayer].StartTime = timePerPlayer[lastPlayer].timeLeft;
        }

        UpdateStartTime(lobbyId);
    }

    public void Pause()
    {
        timePerPlayer[PlayerType.pink].StartTime = timePerPlayer[PlayerType.pink].timeLeft;
        timePerPlayer[PlayerType.blue].StartTime = timePerPlayer[PlayerType.blue].timeLeft;
    }

    public void UpdateStartTime(int lobbyId)
    {
        startTime = TimerUtils.Timestamp();
        BroadcastTimerInfo(lobbyId);
    }

    public void UpdateTime(int lobbyId)
    {
        if (timePerPlayer[currentPlayer].timeLeft > 0)
        {
            timerRanOff = false;

            float timePassed = TimerUtils.TimeSince(startTime);
            timePerPlayer[currentPlayer].timeLeft = timePerPlayer[currentPlayer].StartTime - timePassed;
        }
        else
        {
            if (timerRanOff)
                return;

            timerRanOff = true;
            if (currentTimerType == TimerType.GAMEPLAY)
            {
                timePerPlayer[currentPlayer].debuff++;
            }

            OnlineServer.Instance.Broadcast(new MsgServerNotification
            {
                serverNotification = ServerNotification.TIMEOUT,
                gamePhase = currentGamePhase,
                currentPlayer = currentPlayer
            }, lobbyId);
        }
    }

    public void SyncTimer(int lobbyId, NetworkConnection cnn)
    {
        OnlineServer.Instance.SendToClient(WriteMsgSyncTimer(), cnn, lobbyId);
    }

    private void BroadcastTimerInfo(int lobbyId)
    {
        OnlineServer.Instance.Broadcast(WriteMsgSyncTimer(), lobbyId);
    }

    private MsgSyncTimer WriteMsgSyncTimer()
    {
        return new MsgSyncTimer
        {
            pinkTimeLeft = timePerPlayer[PlayerType.pink].StartTime,
            blueTimeLeft = timePerPlayer[PlayerType.blue].StartTime,
            startTimestamp = startTime
        };
    }
}
