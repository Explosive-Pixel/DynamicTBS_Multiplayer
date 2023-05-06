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
    #region Timer config

    private static readonly Dictionary<TimerType, float> totalTime = new Dictionary<TimerType, float>()
    {
        { TimerType.DRAFT_AND_PLACEMENT, 300 },
        { TimerType.GAMEPLAY, 90 }
    };

    public static Dictionary<TimerType, float> TotalTime { get { return totalTime; } }

    private readonly Dictionary<GamePhase, NoTimeLeftConsequence> noTimeLeftConsequences = new Dictionary<GamePhase, NoTimeLeftConsequence>()
    {
        { GamePhase.DRAFT, (player) => DraftManager.RandomDraft(player) },
        { GamePhase.PLACEMENT, (player) => PlacementManager.RandomPlacement(player) }
    };

    #endregion

    [SerializeField] private GameObject timer;

    private delegate void NoTimeLeftConsequence(Player player);

    public TimerType timerType;
    public GamePhase gamePhase;

    public bool TimerOn = false;
    public Color color_blue;
    public Color color_pink;

    public TMPro.TMP_Text Timertext;

    private readonly Dictionary<Player, float> timeleftPerPlayer = new Dictionary<Player, float>();
    private readonly Dictionary<PlayerType, Color> colorPerPlayer = new Dictionary<PlayerType, Color>();

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        UnsubscribeEvents();

        timer.SetActive(false);
        TimerOn = false;
        colorPerPlayer[PlayerType.blue] = color_blue;
        colorPerPlayer[PlayerType.pink] = color_pink;

        SubscribeEvents();
    }

    private void SetActive(GamePhase gamePhase)
    {
        timerType = gamePhase == GamePhase.GAMEPLAY ? TimerType.GAMEPLAY : TimerType.DRAFT_AND_PLACEMENT;

        foreach (Player player in PlayerManager.GetAllPlayers())
        {
            timeleftPerPlayer[player] = totalTime[timerType];
        }

        TimerOn = true;
        timer.SetActive(true);
        ChangeTextColor(PlayerManager.StartPlayer[gamePhase]);

        GameplayEvents.OnPlayerTurnEnded += ResetTimer;
    }

    private void ResetTimer(Player player)
    {
        Player nextPlayer = PlayerManager.GetOtherPlayer(player);
        ChangeTextColor(nextPlayer.GetPlayerType());
    }

    private void ChangeTextColor(PlayerType side)
    {
        Timertext.color = colorPerPlayer[side];
    }

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += SetActive;
    }

    private void UnsubscribeEvents()
    {
        AudioEvents.OnSpawnMasters -= Init;
        GameEvents.OnGamePhaseStart -= SetActive;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
