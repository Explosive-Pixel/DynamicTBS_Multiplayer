using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TimerScript : MonoBehaviour
{
    #region Timer config

    private const float timePerTurn = 20;
    private const float debuffRate = 0.25f;
    private const int maxDebuffs = 3;

    #endregion

    [SerializeField]
    private Animator lampAnimator;

    [SerializeField]
    private GameObject timer;

    public float Timeleft;
    public bool TimerOn = false;
    public Color Player1;
    public Color Player2;

    public TMPro.TMP_Text Timertext;

    private readonly Dictionary<Player, int> timerDebuffsPerPlayer = new Dictionary<Player, int>();

    private bool Paused = false;

    private bool isHost = true;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        UnsubscribeEvents();

        isHost = GameManager.gameType == GameType.local || (Server.Instance && Server.Instance.IsActive); 

        timer.SetActive(false);
        TimerOn = false;
        Paused = false;

        Timeleft = timePerTurn;

        foreach (Player player in PlayerManager.GetAllPlayers())
        {
            timerDebuffsPerPlayer[player] = 0;
        }

        SubscribeEvents();
    }

    private void SetActive()
    {
        TimerOn = true;
        timer.SetActive(true);
        Timertext.color = GetPlayerColor(PlayerManager.GameplayPhaseStartPlayer);
        GameplayEvents.OnPlayerTurnEnded += ResetTimer;
    }

    private void ResetTimer(Player player)
    {
        Player nextPlayer = PlayerManager.GetOtherPlayer(player);
        Timeleft = timePerTurn * Mathf.Pow(1 - debuffRate, timerDebuffsPerPlayer[nextPlayer]);

        UpdateLamps(timerDebuffsPerPlayer[nextPlayer]);
        ChangeTextColor(nextPlayer.GetPlayerType());
    }

    private void Update()
    {
        if (TimerOn && !Paused)
        {
            if (Timeleft > 0)
            {
                if (isHost)
                {
                    Timeleft -= Time.deltaTime;
                    BroadcastTimerData();
                }
                UpdateTimer(Timeleft);
            }
            else
            {
                DrawConsequences();
            }
        }
    }

    private void ChangeTextColor(PlayerType side)
    {
        Timertext.color = GetPlayerColor(side);
    }

    private Color GetPlayerColor(PlayerType side)
    {
        if (side == PlayerType.blue)
            return Player1;
        return Player2;
    }

    private void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        Timertext.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    private void UpdateLamps(int debuffCount)
    {
        // TODO: if
        // debuffCount = 0 -> set both lamps on
        // debuffCount = 1 -> set one lamp on, one lamp off
        // debuffCount = 2 -> set both lamps off
        lampAnimator.SetInteger("Actions", GameplayManager.maxActionsPerRound - debuffCount);
    }

    private void DrawConsequences()
    {
        Player currentPlayer = PlayerManager.GetCurrentPlayer();
        timerDebuffsPerPlayer[currentPlayer] += 1;
        if (timerDebuffsPerPlayer[currentPlayer] == maxDebuffs)
        {
            GameplayEvents.GameIsOver(PlayerManager.GetOtherPlayer(currentPlayer).GetPlayerType(), GameOverCondition.PLAYER_TIMEOUT);
        }

        GameplayEvents.AbortCurrentPlayerTurn();

        if (GameManager.gameType == GameType.multiplayer && isHost)
        {
            GameplayEvents.ServerActionExecuted(ServerActionType.AbortTurn);
        }
    }

    private void BroadcastTimerData()
    {
        if (GameManager.gameType == GameType.multiplayer && isHost)
        {
            Server.Instance.Broadcast(new NetUpdateTimer()
            {
                currentTime = Timeleft,
                pinkDebuff = timerDebuffsPerPlayer[PlayerManager.PinkPlayer],
                blueDebuff = timerDebuffsPerPlayer[PlayerManager.BluePlayer],
                currentPlayerId = (int)PlayerManager.GetCurrentPlayer().GetPlayerType()
            });
        }
    }

    private void UpdateTimerInfo(NetMessage msg)
    {
        NetUpdateTimer netUpdateTimer = msg as NetUpdateTimer;

        Timeleft = netUpdateTimer.currentTime;
        timerDebuffsPerPlayer[PlayerManager.PinkPlayer] = netUpdateTimer.pinkDebuff;
        timerDebuffsPerPlayer[PlayerManager.BluePlayer] = netUpdateTimer.blueDebuff;
    }

    private void HandlePause(Player player, UIActionType uIActionType)
    {
        if (TimerOn)
        {
            if (uIActionType == UIActionType.PauseGame)
            {
                Paused = true;
            }
            else if (uIActionType == UIActionType.UnpauseGame)
            {
                Paused = false;
            }
        }
    }

    private void ResetAll(PlayerType? winner, GameOverCondition gameOverCondition)
    {
        Init();
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += SetActive;
        GameplayEvents.OnExecuteUIAction += HandlePause;
        GameplayEvents.OnGameOver += ResetAll;

        if(!isHost)
        {
            NetUtility.C_UPDATE_TIMER += UpdateTimerInfo;
        }
    }
    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= SetActive;
        GameplayEvents.OnExecuteUIAction -= HandlePause;
        GameplayEvents.OnGameOver -= ResetAll;
        GameplayEvents.OnPlayerTurnEnded -= ResetTimer;

        NetUtility.C_UPDATE_TIMER -= UpdateTimerInfo;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}