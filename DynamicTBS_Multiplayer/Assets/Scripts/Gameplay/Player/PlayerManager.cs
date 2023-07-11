using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Player Config

    private static readonly Dictionary<GamePhase, PlayerType> startPlayer = new Dictionary<GamePhase, PlayerType>()
    {
        { GamePhase.DRAFT, PlayerType.pink },
        { GamePhase.PLACEMENT, PlayerType.pink },
        { GamePhase.GAMEPLAY, PlayerType.blue }
    };

    #endregion

    private static Player currentPlayer;

    private static Player bluePlayer;
    public static Player BluePlayer { get { return bluePlayer; } }

    private static Player pinkPlayer;
    public static Player PinkPlayer { get { return pinkPlayer; } }

    public static Dictionary<GamePhase, PlayerType> StartPlayer { get { return startPlayer; } }

    private void Awake()
    {
        bluePlayer = new Player(PlayerType.blue);
        pinkPlayer = new Player(PlayerType.pink);
        SubscribeEvents();

        currentPlayer = GetPlayer(startPlayer[GamePhase.DRAFT]);
    }

    public static List<Player> GetAllPlayers()
    {
        return new List<Player>() { bluePlayer, pinkPlayer };
    }

    public static Player GetCurrentlyExecutingPlayer()
    {
        return GameManager.gameType == GameType.ONLINE ? GetPlayer(OnlineClient.Instance.Side) : GetCurrentPlayer();
    }

    public static Player GetOtherPlayer(Player player)
    {
        if (player == bluePlayer)
            return pinkPlayer;
        return bluePlayer;
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

    public static bool IsCurrentPlayer(string name)
    {
        return GetPlayer(name) == GetCurrentPlayer();
    }

    public static bool ClientIsCurrentPlayer()
    {
        return GameManager.gameType == GameType.LOCAL || OnlineClient.Instance.Side == GetCurrentPlayer().GetPlayerType();
    }

    public static Player GetPlayer(string name)
    {
        if (name.ToLower().Contains(PlayerType.blue.ToString()))
        {
            return bluePlayer;
        }
        else if (name.ToLower().Contains(PlayerType.pink.ToString()))
        {
            return pinkPlayer;
        }
        return null;
    }

    public static Player GetPlayer(PlayerType playerType)
    {
        if (playerType == PlayerType.blue)
            return bluePlayer;
        return pinkPlayer;
    }

    private void ResetRoundCounters()
    {
        bluePlayer.ResetRoundCounter();
        pinkPlayer.ResetRoundCounter();
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