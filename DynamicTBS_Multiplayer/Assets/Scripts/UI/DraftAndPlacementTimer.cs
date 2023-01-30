using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftAndPlacementTimer : MonoBehaviour
{
    #region Timer config

    private const float totalTimePerPlayer = 300;
    private readonly NoTimeLeftConsequence noTimeLeftConsequenceDraft = (player) => DraftManager.RandomDraft(player);
    private readonly NoTimeLeftConsequence noTimeLeftConsequencePlacement = (player) => PlacementManager.RandomPlacement(player);

    #endregion

    [SerializeField] private GameObject timer;

    private delegate void NoTimeLeftConsequence(Player player);

    // set false if it is PlacementTimer
    public bool isDraftTimer;

    public bool TimerOn = false;
    public Color Player1;
    public Color Player2;

    public TMPro.TMP_Text Timertext;

    private readonly Dictionary<Player, float> timeleftPerPlayer = new Dictionary<Player, float>();
    private NoTimeLeftConsequence noTimeLeftConsequence;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        UnsubscribeEvents();

        timer.SetActive(false);
        TimerOn = false;

        noTimeLeftConsequence = isDraftTimer ? noTimeLeftConsequenceDraft : noTimeLeftConsequencePlacement;
        SubscribeEvents();
    }

    private void SetActive()
    {
        foreach (Player player in PlayerManager.GetAllPlayers())
        {
            timeleftPerPlayer[player] = totalTimePerPlayer;
        }

        TimerOn = true;
        timer.SetActive(true);
        PlayerType startPlayer = isDraftTimer ? PlayerManager.DraftPhaseStartPlayer : PlayerManager.PlacementPhaseStartPlayer; 
        Timertext.color = GetPlayerColor(startPlayer);
        GameplayEvents.OnPlayerTurnEnded += ResetTimer;
    }

    private void ResetTimer(Player player)
    {
        Player nextPlayer = PlayerManager.GetOtherPlayer(player);
        ChangeTextColor(nextPlayer.GetPlayerType());
    }

    private void Update()
    {
        if (TimerOn && !GameplayManager.gameIsPaused)
        {
            Player player = PlayerManager.GetCurrentPlayer();
            if (timeleftPerPlayer[player] > 0)
            {
                if (GameManager.IsHost())
                {
                    timeleftPerPlayer[player] -= Time.deltaTime;
                    BroadcastTimerData();
                }
                UpdateTimer(timeleftPerPlayer[player]);
            }
            else
            {
                if(player == PlayerManager.GetCurrentlyExecutingPlayer())
                {
                    noTimeLeftConsequence(player);
                }
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

    private void BroadcastTimerData()
    {
        if (GameManager.IsMultiplayerHost())
        {
            Server.Instance.Broadcast(new NetUpdateTimer()
            {
                pinkTimeLeft = timeleftPerPlayer[PlayerManager.PinkPlayer],
                blueTimeLeft = timeleftPerPlayer[PlayerManager.BluePlayer]
            });
        }
    }

    private void UpdateTimerInfo(NetMessage msg)
    {
        NetUpdateTimer netUpdateTimer = msg as NetUpdateTimer;

        timeleftPerPlayer[PlayerManager.PinkPlayer] = netUpdateTimer.pinkTimeLeft;
        timeleftPerPlayer[PlayerManager.BluePlayer] = netUpdateTimer.blueTimeLeft;
    }

    private void SubscribeEvents()
    {
        if (isDraftTimer)
        {
            DraftEvents.OnStartDraft += SetActive;
            DraftEvents.OnEndDraft += Init;
        }
        else
        {
            DraftEvents.OnEndDraft += SetActive;
            GameplayEvents.OnGameplayPhaseStart += Init;
        }

        if (!GameManager.IsHost())
        {
            NetUtility.C_UPDATE_TIMER += UpdateTimerInfo;
        }
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnStartDraft -= SetActive;
        DraftEvents.OnEndDraft -= Init;
        DraftEvents.OnEndDraft -= SetActive;
        GameplayEvents.OnGameplayPhaseStart -= Init;
        GameplayEvents.OnPlayerTurnEnded -= ResetTimer;
        NetUtility.C_UPDATE_TIMER -= UpdateTimerInfo;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
