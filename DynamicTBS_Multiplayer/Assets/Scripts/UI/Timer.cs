using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimerType
{
    DRAFT_AND_PLACEMENT,
    GAMEPLAY
}

public class Timer : MonoBehaviour
{
    private class PlayerInfo
    {
        public float timeLeft;
        public int debuff;
        public Color color;

        public PlayerInfo(Color color, float timeLeft)
        {
            this.timeLeft = timeLeft;
            this.color = color;
            debuff = 0;
        }
    }

    #region Timer config

    private static readonly Dictionary<TimerType, float> totalTime = new Dictionary<TimerType, float>()
    {
        { TimerType.DRAFT_AND_PLACEMENT, 300 },
        { TimerType.GAMEPLAY, 90 }
    };

    public static Dictionary<TimerType, float> TotalTime { get { return totalTime; } }

    public const float debuffRate = 0.25f;
    public const int maxDebuffs = 3;

    #endregion

    [SerializeField] private GameObject timer;

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
    private bool IsActive { get { return isActive && !GameplayManager.gameIsPaused; } }

    private void Awake()
    {
        SubscribeEvents();
    }

    public static void InitTime(float draftAndPlacementTime, float gameplayTime)
    {
        totalTime[TimerType.DRAFT_AND_PLACEMENT] = draftAndPlacementTime;
        totalTime[TimerType.GAMEPLAY] = gameplayTime;
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

        Player player = PlayerManager.GetCurrentPlayer();
        PlayerType side = player.GetPlayerType();
        if (playerStats[side].timeLeft > 0)
        {
           // if(GameManager.gameType == GameType.local)
           // {
            playerStats[side].timeLeft -= Time.deltaTime;
            //}
            UpdateTime(playerStats[side].timeLeft);
        }
        else
        {
            if (GameManager.gameType == GameType.LOCAL)
            {
                GameplayEvents.TimerTimedOut(gamePhase, player.GetPlayerType(), playerStats[side].debuff);
            }
        }
    }

    private void SetActive(GamePhase gamePhase)
    {
        if (this.gamePhase != gamePhase)
            return;

        timerType = gamePhase == GamePhase.GAMEPLAY ? TimerType.GAMEPLAY : TimerType.DRAFT_AND_PLACEMENT;

        playerStats[PlayerType.pink] = new PlayerInfo(color_pink, totalTime[timerType]);
        playerStats[PlayerType.blue] = new PlayerInfo(color_blue, totalTime[timerType]);

        isActive = true;
        timer.SetActive(true);
        ChangeTextColor(PlayerManager.StartPlayer[gamePhase]);

        GameplayEvents.OnTimerUpdate += UpdateData;
        GameplayEvents.OnTimerTimeout += DrawNoTimeLeftConsequences;

        if (timerType == TimerType.GAMEPLAY)
            GameplayEvents.OnPlayerTurnEnded += ResetTimer;
    }

    private void UpdateData(float pinkTimeLeft, float blueTimeLeft, int pinkDebuff, int blueDebuff)
    {
        playerStats[PlayerType.pink].timeLeft = pinkTimeLeft;
        playerStats[PlayerType.blue].timeLeft = blueTimeLeft;
        playerStats[PlayerType.pink].debuff = pinkDebuff;
        playerStats[PlayerType.blue].debuff = blueDebuff;
    }

    private void UpdateTime(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        Timertext.text = string.Format("{0:00} : {1:00}", minutes, seconds);

        ChangeTextColor(PlayerManager.GetCurrentPlayer().GetPlayerType());
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
        PlayerType nextPlayer = PlayerManager.GetOtherSide(player.GetPlayerType());
        UpdateLamps(playerStats[nextPlayer].debuff);

        playerStats[nextPlayer].timeLeft = totalTime[timerType] * Mathf.Pow(1 - debuffRate, playerStats[nextPlayer].debuff);
    }

    private void ChangeTextColor(PlayerType side)
    {
        Timertext.color = playerStats[side].color;
    }

    private void DrawNoTimeLeftConsequences(GamePhase gamePhase, PlayerType playerType, int currentPlayerDebuff)
    {
        playerStats[playerType].timeLeft = 0;
        AudioEvents.TimeRanOut();
        Player player = PlayerManager.GetPlayer(playerType);
        switch(gamePhase)
        {
            case GamePhase.DRAFT:
                DrawNoTimeLeftConsequences_Draft(player);
                break;
            case GamePhase.PLACEMENT:
                DrawNoTimeLeftConsequences_Placement(player);
                break;
            case GamePhase.GAMEPLAY:
                DrawNoTimeLeftConsequences_Gameplay(player, currentPlayerDebuff);
                break;
        }
    }

    private void DrawNoTimeLeftConsequences_Draft(Player player)
    {
        if (GameManager.gameType == GameType.ONLINE && (OnlineClient.Instance.Side != player.GetPlayerType() || OnlineClient.Instance.IsLoadingGame))
            return;

        DraftManager.RandomDraft(player);
    }

    private void DrawNoTimeLeftConsequences_Placement(Player player)
    {
        if (GameManager.gameType == GameType.ONLINE && (OnlineClient.Instance.Side != player.GetPlayerType() || OnlineClient.Instance.IsLoadingGame))
            return;

        PlacementManager.RandomPlacement(player);
    }

    private void DrawNoTimeLeftConsequences_Gameplay(Player player, int currentPlayerDebuff)
    {
        PlayerType side = player.GetPlayerType();

        if(GameManager.gameType == GameType.LOCAL)
            playerStats[side].debuff += 1;

        int debuff = GameManager.gameType == GameType.LOCAL ? playerStats[side].debuff : currentPlayerDebuff;

        playerStats[side].debuff = debuff;
        if (debuff == maxDebuffs)
        {
            GameplayEvents.GameIsOver(PlayerManager.GetOtherSide(side), GameOverCondition.PLAYER_TIMEOUT);
        }

        GameplayEvents.AbortCurrentPlayerTurn(GameplayManager.GetRemainingActions(), AbortTurnCondition.PLAYER_TIMEOUT);
    }

    private void SetInactive(GamePhase gamePhase)
    {
        if(this.gamePhase == gamePhase)
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
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
