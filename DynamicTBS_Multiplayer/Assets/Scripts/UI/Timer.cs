using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System;

public enum TimerType
{
    DRAFT_AND_PLACEMENT,
    GAMEPLAY
}

public enum TimerSetupType
{
    [Description("Fast")]
    FAST = 1,
    [Description("Standard")]
    STANDARD = 2,
    [Description("Slow")]
    SLOW = 3
}

public class Timer : MonoBehaviour
{
    private class PlayerInfo
    {
        private float startTimeLeft;
        public float timeLeft;
        public int debuff;
        public Color color;

        public float StartTimeLeft { get { return startTimeLeft; } set { startTimeLeft = value; timeLeft = value; } }

        public PlayerInfo(Color color, float startTime)
        {
            StartTimeLeft = startTime;
            this.color = color;
            debuff = 0;
        }
    }

    #region Timer config

    private static readonly Dictionary<TimerSetupType, Dictionary<TimerType, float>> timeSetups = new()
    {
        { TimerSetupType.FAST,
            new() {
                { TimerType.DRAFT_AND_PLACEMENT, 120 },
                { TimerType.GAMEPLAY, 60 }
            }
        },
        {
            TimerSetupType.STANDARD,
            new()
            {
                { TimerType.DRAFT_AND_PLACEMENT, 300 },
                { TimerType.GAMEPLAY, 90 }
            }
        },
        {
            TimerSetupType.SLOW,
            new()
            {
                { TimerType.DRAFT_AND_PLACEMENT, 420 },
                { TimerType.GAMEPLAY, 120 }
            }
        }
    };

    public static Dictionary<TimerType, float> GetTimeSetup(TimerSetupType timerSetup)
    {
        return timeSetups[timerSetup];
    }

    private static TimerSetupType timerSetupType = TimerSetupType.STANDARD;
    public static TimerSetupType TimerSetupType { get { return timerSetupType; } }

    public static Dictionary<TimerType, float> TotalTime { get { return timeSetups[timerSetupType]; } }

    public const float debuffRate = 0.25f;
    public const int maxDebuffs = 3;

    #endregion

    [SerializeField] private GameObject timer;
    [SerializeField] private DraftManager draftManager;

    public TimerType timerType;
    public GamePhase gamePhase;

    public Color color_blue;
    public Color color_pink;
    public TMPro.TMP_Text Timertext;
    public GameObject lamp1;
    public GameObject lamp2;

    private delegate void NoTimeLeftConsequence(Player player);

    private readonly Dictionary<PlayerType, PlayerInfo> playerStats = new Dictionary<PlayerType, PlayerInfo>();

    private bool isActive = false;
    private bool IsActive { get { return isActive && !GameplayManager.gameIsPaused && startTime != null; } }

    private DateTime? startTime = null;
    private bool timerRanOff = false;

    private float ServerTimeDiff { get { return GameManager.gameType == GameType.ONLINE && OnlineClient.Instance && OnlineClient.Instance.IsActive ? OnlineClient.Instance.ServerTimeDiff : 0; } }

    private void Awake()
    {
        SubscribeEvents();
    }

    public static void InitTime(TimerSetupType timerSetup)
    {
        timerSetupType = timerSetup;
    }

    private void SetInactive()
    {
        timer.SetActive(false);
        isActive = false;

        GameplayEvents.OnPlayerTurnEnded -= ResetTimer;
        GameplayEvents.OnTimerUpdate -= UpdateData;
        GameplayEvents.OnTimerTimeout -= DrawNoTimeLeftConsequences;
    }

    private void Update()
    {
        if (!IsActive)
            return;

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        Player player = PlayerManager.GetCurrentPlayer();
        PlayerType side = player.GetPlayerType();
        if (playerStats[side].timeLeft > 0)
        {
            timerRanOff = false;

            float timePassed = TimerUtils.TimeSince(startTime.Value) - ServerTimeDiff;
            playerStats[side].timeLeft = playerStats[side].StartTimeLeft - timePassed;

            PrintTime(playerStats[side].timeLeft);
        }
        else
        {
            if (timerRanOff)
                return;

            timerRanOff = true;
            if (GameManager.gameType == GameType.LOCAL)
            {
                GameplayEvents.TimerTimedOut(gamePhase, player.GetPlayerType());
            }
        }
    }

    private void SetActive(GamePhase gamePhase)
    {
        if (this.gamePhase != gamePhase)
            return;

        timerType = gamePhase == GamePhase.GAMEPLAY ? TimerType.GAMEPLAY : TimerType.DRAFT_AND_PLACEMENT;

        playerStats[PlayerType.pink] = new PlayerInfo(color_pink, TotalTime[timerType]);
        playerStats[PlayerType.blue] = new PlayerInfo(color_blue, TotalTime[timerType]);

        GameplayEvents.OnTimerUpdate += UpdateData;
        GameplayEvents.OnTimerTimeout += DrawNoTimeLeftConsequences;
        GameplayEvents.OnPlayerTurnEnded += ResetTimer;
        GameplayEvents.OnGamePause += OnUnpauseGame;

        ResetTimer(PlayerManager.StartPlayer[gamePhase]);

        isActive = true;
        timer.SetActive(true);
    }

    private void OnUnpauseGame(bool paused)
    {
        if (GameManager.gameType != GameType.LOCAL)
            return;

        if (!paused)
        {
            UpdateData(playerStats[PlayerType.pink].timeLeft, playerStats[PlayerType.blue].timeLeft, TimerUtils.Timestamp());
        }
    }

    private void UpdateData(float pinkTimeLeft, float blueTimeLeft, DateTime startTime)
    {
        playerStats[PlayerType.pink].StartTimeLeft = pinkTimeLeft;
        playerStats[PlayerType.blue].StartTimeLeft = blueTimeLeft;

        this.startTime = startTime;
        UpdateTimer();
    }

    private void PrintTime(float currentTime)
    {
        currentTime = currentTime < 0 ? 0 : currentTime;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        Timertext.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    private void UpdateLamps(int debuffCount)
    {
        if (lamp1 == null || lamp2 == null)
            return;

        lamp1.GetComponent<Animator>().SetInteger("Actions", debuffCount > 0 ? 1 : 0);
        lamp2.GetComponent<Animator>().SetInteger("Actions", debuffCount > 1 ? 1 : 0);
    }

    private void ResetTimer(Player player)
    {
        Player nextPlayer = PlayerManager.GetOtherPlayer(player);
        ResetTimer(nextPlayer.GetPlayerType());
    }

    private void ResetTimer(PlayerType nextPlayer)
    {
        startTime = null;
        timerRanOff = false;

        if (timerType == TimerType.GAMEPLAY)
        {
            UpdateLamps(playerStats[nextPlayer].debuff);
            playerStats[nextPlayer].StartTimeLeft = TotalTime[timerType] * Mathf.Pow(1 - debuffRate, playerStats[nextPlayer].debuff);
        }
        else
        {
            PlayerType lastPlayer = PlayerManager.GetOtherSide(nextPlayer);
            playerStats[lastPlayer].StartTimeLeft = playerStats[lastPlayer].timeLeft;
        }

        PrintTime(playerStats[nextPlayer].timeLeft);
        ChangeTextColor(nextPlayer);

        if (GameManager.gameType == GameType.LOCAL)
            startTime = TimerUtils.Timestamp();
    }

    private void ChangeTextColor(PlayerType side)
    {
        Timertext.color = playerStats[side].color;
    }

    private void DrawNoTimeLeftConsequences(GamePhase gamePhase, PlayerType playerType)
    {
        playerStats[playerType].timeLeft = 0;
        AudioEvents.TimeRanOut();

        switch (gamePhase)
        {
            case GamePhase.DRAFT:
                DrawNoTimeLeftConsequences_Draft(playerType);
                break;
            case GamePhase.PLACEMENT:
                DrawNoTimeLeftConsequences_Placement(playerType);
                break;
            case GamePhase.GAMEPLAY:
                DrawNoTimeLeftConsequences_Gameplay(playerType);
                break;
        }
    }

    private void DrawNoTimeLeftConsequences_Draft(PlayerType playerType)
    {
        if (GameManager.gameType == GameType.ONLINE && (OnlineClient.Instance.Side != playerType || OnlineClient.Instance.IsLoadingGame))
            return;

        draftManager.RandomDrafts(playerType);
    }

    private void DrawNoTimeLeftConsequences_Placement(PlayerType playerType)
    {
        if (GameManager.gameType == GameType.ONLINE && (OnlineClient.Instance.Side != playerType || OnlineClient.Instance.IsLoadingGame))
            return;

        Player player = PlayerManager.GetPlayer(playerType);
        PlacementManager.RandomPlacements(player);
    }

    private void DrawNoTimeLeftConsequences_Gameplay(PlayerType side)
    {
        playerStats[side].debuff += 1;

        if (playerStats[side].debuff == maxDebuffs)
        {
            GameplayEvents.GameIsOver(PlayerManager.GetOtherSide(side), GameOverCondition.PLAYER_TIMEOUT);
        }

        GameplayEvents.AbortCurrentPlayerTurn(GameplayManager.GetRemainingActions(), AbortTurnCondition.PLAYER_TIMEOUT);
    }

    private void SetInactive(GamePhase gamePhase)
    {
        if (this.gamePhase == gamePhase)
            SetInactive();
    }

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += SetActive;
        GameEvents.OnGamePhaseEnd += SetInactive;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= SetActive;
        GameEvents.OnGamePhaseEnd -= SetInactive;
        GameplayEvents.OnPlayerTurnEnded -= ResetTimer;
        GameplayEvents.OnTimerUpdate -= UpdateData;
        GameplayEvents.OnTimerTimeout -= DrawNoTimeLeftConsequences;
        GameplayEvents.OnGamePause -= OnUnpauseGame;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
