using System;

public class GameTimer : BaseTimer
{
    private void Awake()
    {
        SubscribeEvents();
    }

    private void SetActive(PlayerType currentPlayer, DateTime startTime)
    {
        if (currentPlayer == playerType)
            SetActive(startTime);
        else
            SetInactive();
    }

    private void UpdateTimerLocal(PlayerType currentPlayer)
    {
        if (GameManager.GameType == GameType.LOCAL && GameManager.CurrentGamePhase == gamePhase)
            SetActive(currentPlayer, TimerUtils.Timestamp());
    }

    private void UpdateTimerOnline(float pinkTimeLeft, float blueTimeLeft, DateTime startTimestamp, GamePhase gamePhase, PlayerType currentPlayer)
    {
        if (GameManager.GameType == GameType.ONLINE && this.gamePhase == gamePhase)
        {
            UpdateTimeleft(pinkTimeLeft, blueTimeLeft);
            SetActive(currentPlayer, startTimestamp);
        }
    }

    private void DrawNoTimeLeftConsequences(GamePhase gamePhase, PlayerType playerType)
    {
        if (this.gamePhase != gamePhase || this.playerType != playerType)
            return;

        DrawNoTimeLeftConsequences();
    }

    private void PauseTimerLocal(bool paused)
    {
        if (GameManager.GameType == GameType.LOCAL)
        {
            if (paused)
            {
                PauseTimer();
            }
            else
                UnpauseTimer(TimerUtils.Timestamp());
        }
    }

    private void PauseTimerOnline(WSMsgPauseGame msg)
    {
        if (msg.timerUpdate.gamePhase == gamePhase && msg.timerUpdate.currentPlayer == playerType)
        {
            UpdateTimeleft(msg.timerUpdate.pinkTimeLeft, msg.timerUpdate.blueTimeLeft);
            if (msg.pause)
                PauseTimer();
            else
            {
                UnpauseTimer(TimerUtils.UnixTimeStampToDateTime(msg.timerUpdate.startTimestamp));
            }
        }
    }

    private void SetInactive(GamePhase gamePhase)
    {
        if (this.gamePhase == gamePhase)
        {
            Destroy(this);
        }
    }

    private void UpdateTimeleft(float pinkTimeLeft, float blueTimeLeft)
    {
        timeleft = playerType == PlayerType.pink ? pinkTimeLeft : blueTimeLeft;
        UpdateUI();
    }

    private void SubscribeEvents()
    {
        GameplayEvents.OnCurrentPlayerChanged += UpdateTimerLocal;
        GameplayEvents.OnTimerUpdate += UpdateTimerOnline;
        GameplayEvents.OnTimerTimeout += DrawNoTimeLeftConsequences;
        GameplayEvents.OnGamePause += PauseTimerLocal;
        GameplayEvents.OnGamePauseOnline += PauseTimerOnline;
        GameEvents.OnGamePhaseEnd += SetInactive;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnCurrentPlayerChanged -= UpdateTimerLocal;
        GameplayEvents.OnTimerUpdate -= UpdateTimerOnline;
        GameplayEvents.OnTimerTimeout -= DrawNoTimeLeftConsequences;
        GameplayEvents.OnGamePause -= PauseTimerLocal;
        GameplayEvents.OnGamePauseOnline -= PauseTimerOnline;
        GameEvents.OnGamePhaseEnd -= SetInactive;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
