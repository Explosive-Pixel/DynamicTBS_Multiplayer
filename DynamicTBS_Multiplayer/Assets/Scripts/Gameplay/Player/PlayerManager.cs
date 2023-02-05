using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Player Config

    private const PlayerType draftPhaseStartPlayer = PlayerType.pink;
    private const PlayerType placementPhaseStartPlayer = PlayerType.pink;
    private const PlayerType gameplayPhaseStartPlayer = PlayerType.blue;

    #endregion

    private static Player currentPlayer;

    private static Player bluePlayer;
    public static Player BluePlayer { get { return bluePlayer; } }

    private static Player pinkPlayer;
    public static Player PinkPlayer { get { return pinkPlayer; } }

    public static PlayerType DraftPhaseStartPlayer { get { return draftPhaseStartPlayer; } }
    public static PlayerType PlacementPhaseStartPlayer { get { return placementPhaseStartPlayer; } }
    public static PlayerType GameplayPhaseStartPlayer { get { return gameplayPhaseStartPlayer; } }

    private void Awake()
    {
        bluePlayer = new Player(PlayerType.blue);
        pinkPlayer = new Player(PlayerType.pink);
        SubscribeEvents();

        currentPlayer = GetPlayer(draftPhaseStartPlayer);
    }

    public static List<Player> GetAllPlayers()
    {
        return new List<Player>() { bluePlayer, pinkPlayer };
    }

    public static Player GetCurrentlyExecutingPlayer()
    {
        return GameManager.gameType == GameType.multiplayer ? GetPlayer(Client.Instance.side) : GetCurrentPlayer();
    }

    public static Player GetOtherPlayer(Player player)
    {
        if (player == bluePlayer)
            return pinkPlayer;
        return bluePlayer;
    }

    public static PlayerType GetOtherSide(PlayerType side)
    {
        if(side == PlayerType.blue)
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
    }

    public static Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public static bool IsCurrentPlayer(string name)
    {
        return GetPlayer(name) == GetCurrentPlayer();
    }

    public static bool ClientIsCurrentPlayer()
    {
        return GameManager.gameType == GameType.local || Client.Instance.side == GetCurrentPlayer().GetPlayerType();
    }

    public static Player GetPlayer(string name)
    {
        if(name.ToLower().Contains(PlayerType.blue.ToString()))
        {
            return bluePlayer;
        } else if(name.ToLower().Contains(PlayerType.pink.ToString()))
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

    private void SetPlacementPhaseStartPlayer()
    {
        currentPlayer = GetPlayer(placementPhaseStartPlayer);
    }

    private void SetGameplayPhaseStartPlayer()
    {
        currentPlayer = GetPlayer(gameplayPhaseStartPlayer);
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        DraftEvents.OnEndDraft += SetPlacementPhaseStartPlayer;
        GameplayEvents.OnGameplayPhaseStart += ResetRoundCounters;
        GameplayEvents.OnGameplayPhaseStart += SetGameplayPhaseStartPlayer;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnEndDraft -= SetPlacementPhaseStartPlayer;
        GameplayEvents.OnGameplayPhaseStart -= ResetRoundCounters;
        GameplayEvents.OnGameplayPhaseStart -= SetGameplayPhaseStartPlayer;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}