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
        { TimerType.DRAFT_AND_PLACEMENT, 2 },
        { TimerType.GAMEPLAY, 90 }
    };

    public static Dictionary<TimerType, float> TotalTime { get { return totalTime; } }

    private const float debuffRate = 0.25f;
    private const int maxDebuffs = 3;

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

    private void SetInactive()
    {
        timer.SetActive(false);
        isActive = false;

        GameplayEvents.OnPlayerTurnEnded -= ResetTimer;
    }

    private void Update()
    {
        if (!IsActive)
            return;

        Player player = PlayerManager.GetCurrentPlayer();
        PlayerType side = player.GetPlayerType();
        if (playerStats[side].timeLeft > 0)
        {
            playerStats[side].timeLeft -= Time.deltaTime;
            UpdateTime(playerStats[side].timeLeft);
        }
        else
        {
            AudioEvents.TimeRanOut();
            if (player == PlayerManager.GetCurrentlyExecutingPlayer())
            {
                DrawNoTimeLeftConsequences(gamePhase, player);
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

        GameplayEvents.OnPlayerTurnEnded += ResetTimer;
    }

    private void UpdateTime(float currentTime)
    {
        currentTime += 1;

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
        PlayerType nextPlayer = PlayerManager.GetOtherSide(player.GetPlayerType());

        if (timerType == TimerType.GAMEPLAY)
        {
            playerStats[nextPlayer].timeLeft = totalTime[timerType] * Mathf.Pow(1 - debuffRate, playerStats[nextPlayer].debuff);
            UpdateLamps(playerStats[nextPlayer].debuff);
        }

        ChangeTextColor(nextPlayer);
    }

    private void ChangeTextColor(PlayerType side)
    {
        Timertext.color = playerStats[side].color;
    }

    private void DrawNoTimeLeftConsequences(GamePhase gamePhase, Player player)
    {
        switch(gamePhase)
        {
            case GamePhase.DRAFT:
                DrawNoTimeLeftConsequences_Draft(player);
                break;
            case GamePhase.PLACEMENT:
                DrawNoTimeLeftConsequences_Placement(player);
                break;
            case GamePhase.GAMEPLAY:
                DrawNoTimeLeftConsequences_Gameplay(player);
                break;
        }
    }

    private void DrawNoTimeLeftConsequences_Draft(Player player)
    {
        DraftManager.RandomDraft(player);
    }

    private void DrawNoTimeLeftConsequences_Placement(Player player)
    {
        PlacementManager.RandomPlacement(player);
    }

    private void DrawNoTimeLeftConsequences_Gameplay(Player player)
    {
        PlayerType side = player.GetPlayerType();

        playerStats[side].debuff += 1;
        if (playerStats[side].debuff == maxDebuffs)
        {
            GameplayEvents.GameIsOver(PlayerManager.GetOtherSide(side), GameOverCondition.PLAYER_TIMEOUT);
        }

        GameplayEvents.AbortCurrentPlayerTurn(GameplayManager.GetRemainingActions(), AbortTurnCondition.PLAYER_TIMEOUT);

       /* if (GameManager.IsMultiplayerHost())
        {
            GameplayEvents.ServerActionExecuted(ServerActionType.AbortTurn);
        } */
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
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
