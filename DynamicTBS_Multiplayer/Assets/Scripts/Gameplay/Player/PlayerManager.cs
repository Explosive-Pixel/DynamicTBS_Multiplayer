using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerType draftStartPlayer;
    [SerializeField] private PlayerType placementStartPlayer;
    [SerializeField] private PlayerType gameplayStartPlayer;

    private static Player currentPlayer;

    private static Dictionary<GamePhase, PlayerType> startPlayer;
    public static Dictionary<GamePhase, PlayerType> StartPlayer { get { return startPlayer; } }

    private static readonly Dictionary<PlayerType, Player> players = new();

    private void Awake()
    {
        startPlayer = new Dictionary<GamePhase, PlayerType>()
        {
            { GamePhase.DRAFT, draftStartPlayer },
            { GamePhase.PLACEMENT, placementStartPlayer },
            { GamePhase.GAMEPLAY, gameplayStartPlayer }
        };

        players.Add(PlayerType.blue, new Player(PlayerType.blue));
        players.Add(PlayerType.pink, new Player(PlayerType.pink));
        SubscribeEvents();

        currentPlayer = GetPlayer(startPlayer[GamePhase.DRAFT]);
    }

    public static List<Player> GetAllPlayers()
    {
        return players.Values.ToList();
    }

    public static Player GetCurrentlyExecutingPlayer()
    {
        return GameManager.gameType == GameType.ONLINE ? GetPlayer(OnlineClient.Instance.Side) : GetCurrentPlayer();
    }

    public static Player GetOtherPlayer(Player player)
    {
        if (player == players[PlayerType.blue])
            return players[PlayerType.pink];
        return players[PlayerType.blue];
    }

    public static PlayerType GetOtherSide(PlayerType side)
    {
        if (side == PlayerType.blue)
        {
            return PlayerType.pink;
        }
        return PlayerType.blue;
    }

    public static void NextPlayer()
    {
        currentPlayer.IncreaseRoundCounter();
        GameplayEvents.EndPlayerTurn(currentPlayer);

        currentPlayer = GetOtherPlayer(currentPlayer);
        CurrentPlayerChanged();
    }

    public static Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public static bool IsCurrentPlayer(PlayerType side)
    {
        return side == GetCurrentPlayer().GetPlayerType();
    }

    public static bool ClientIsCurrentPlayer()
    {
        return GameManager.gameType == GameType.LOCAL || OnlineClient.Instance.Side == GetCurrentPlayer().GetPlayerType();
    }

    public static Player GetPlayer(PlayerType side)
    {
        return players[side];
    }

    private void ResetRoundCounters()
    {
        GetAllPlayers().ForEach(player => player.ResetRoundCounter());
    }

    private void SetStartPlayer(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.NONE)
            return;

        currentPlayer = GetPlayer(startPlayer[gamePhase]);

        if (gamePhase == GamePhase.GAMEPLAY)
        {
            ResetRoundCounters();
        }

        CurrentPlayerChanged();
    }

    private static void CurrentPlayerChanged()
    {
        GameplayEvents.ChangeCurrentPlayer(currentPlayer.GetPlayerType());
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += SetStartPlayer;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= SetStartPlayer;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}