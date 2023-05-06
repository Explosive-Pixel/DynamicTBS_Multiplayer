using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GameplayTimer : MonoBehaviour
{
    #region Timer config

    private const float timePerTurn = 90;
    public static float DefaultTimePerTurn { get { return timePerTurn; } }

    private const float debuffRate = 0.25f;
    private const int maxDebuffs = 3;

    #endregion

    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject lamp1;
    [SerializeField] private GameObject lamp2;

    public float Timeleft;
    public bool TimerOn = false;
    public Color Player1;
    public Color Player2;

    public TMPro.TMP_Text Timertext;

    private readonly Dictionary<Player, int> timerDebuffsPerPlayer = new Dictionary<Player, int>();

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        UnsubscribeEvents();

        timer.SetActive(false);
        TimerOn = false;

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
       // Timertext.color = GetPlayerColor(PlayerManager.GameplayPhaseStartPlayer);
        GameplayEvents.OnPlayerTurnEnded += ResetTimer;

        if (!GameManager.IsHost())
        {
            NetUtility.C_UPDATE_TIMER += UpdateTimerInfo;
        }
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
        if (TimerOn && !GameplayManager.gameIsPaused)
        {
            if (Timeleft > 0)
            {
                if (GameManager.IsHost())
                {
                    Timeleft -= Time.deltaTime;
                    BroadcastTimerData();
                }
                UpdateTimer(Timeleft);
            }
            else
            {
                AudioEvents.TimeRanOut();
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
        lamp1.GetComponent<Animator>().SetInteger("Actions", debuffCount > 0 ? 1 : 0);
        lamp2.GetComponent<Animator>().SetInteger("Actions", debuffCount > 1 ? 1 : 0);
    }

    private void DrawConsequences()
    {
        Player currentPlayer = PlayerManager.GetCurrentPlayer();
        timerDebuffsPerPlayer[currentPlayer] += 1;
        if (timerDebuffsPerPlayer[currentPlayer] == maxDebuffs)
        {
            GameplayEvents.GameIsOver(PlayerManager.GetOtherPlayer(currentPlayer).GetPlayerType(), GameOverCondition.PLAYER_TIMEOUT);
        }

        GameplayEvents.AbortCurrentPlayerTurn(GameplayManager.GetRemainingActions(), AbortTurnCondition.PLAYER_TIMEOUT);

        if (GameManager.IsMultiplayerHost())
        {
            GameplayEvents.ServerActionExecuted(ServerActionType.AbortTurn);
        }
    }

    private void BroadcastTimerData()
    {
        if (GameManager.IsMultiplayerHost())
        {
            Server.Instance.Broadcast(new NetUpdateTimer()
            {
                currentTime = Timeleft,
                pinkDebuff = timerDebuffsPerPlayer[PlayerManager.PinkPlayer],
                blueDebuff = timerDebuffsPerPlayer[PlayerManager.BluePlayer]
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

    private void ResetAll(PlayerType? winner, GameOverCondition gameOverCondition)
    {
        Init();
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
       // GameplayEvents.OnGameplayPhaseStart += SetActive;
        GameplayEvents.OnGameOver += ResetAll;
    }
    private void UnsubscribeEvents()
    {
        //GameplayEvents.OnGameplayPhaseStart -= SetActive;
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