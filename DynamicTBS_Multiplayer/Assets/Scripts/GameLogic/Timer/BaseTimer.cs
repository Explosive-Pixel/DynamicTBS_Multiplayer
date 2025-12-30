using System;
using UnityEngine;

public class BaseTimer : MonoBehaviour
{
    protected GamePhase gamePhase;
    protected PlayerType playerType;
    protected float originalDuration;

    protected DateTime startTime;
    protected float duration;
    protected float timeleft;

    public float Timeleft { get { return timeleft; } set { timeleft = value; } }

    protected bool isRunning = false;
    protected bool isPaused = false;

    private float ServerTimeDiff { get { return GameManager.GameType == GameType.ONLINE ? Client.ServerTimeDiff : 0; } }

    void Update()
    {
        if (isRunning && !isPaused)
        {
            Debug.Log("Timer ist running: GamePhase: " + gamePhase + ", PlayerType: " + playerType);
            float timePassed = TimerUtils.TimeSince(startTime) - ServerTimeDiff;
            timeleft = duration - timePassed;

            UpdateUI();

            if (timeleft <= 0)
            {
                if (GameManager.GameType == GameType.LOCAL)
                {
                    GameplayEvents.TimerTimedOut(gamePhase, playerType);
                }

                SetInactive();
            }
        }
    }

    public void Init(GamePhase gamePhase, PlayerType playerType, float originalDuration)
    {
        this.gamePhase = gamePhase;
        this.playerType = playerType;
        this.originalDuration = originalDuration;

        isRunning = false;
        isPaused = false;
    }

    public virtual void SetActive(DateTime startTime) { }

    public virtual void SetInactive() { }

    public virtual void DrawNoTimeLeftConsequences() { }

    public void StartTimer(DateTime startTime, float duration)
    {
        this.startTime = startTime;
        this.duration = duration;
        timeleft = this.duration;

        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void PauseTimer()
    {
        if (isRunning)
            isPaused = true;
    }

    public void UnpauseTimer(DateTime startTime)
    {
        if (isRunning)
        {
            StartTimer(startTime, timeleft);
            isPaused = false;
        }
    }

    protected void UpdateUI()
    {
        float currentTime = timeleft < 0 ? 0 : timeleft;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        gameObject.GetComponent<TMPro.TextMeshPro>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
