using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private const PlayerType draftPhaseStartPlayer = PlayerType.pink;
    private const PlayerType placementPhaseStartPlayer = PlayerType.pink;
    private const PlayerType gameplayPhaseStartPlayer = PlayerType.blue;
    
    private Player currentPlayer;

    private Player bluePlayer;
    private Player pinkPlayer;

    private void Awake()
    {
        bluePlayer = new Player(PlayerType.blue);
        pinkPlayer = new Player(PlayerType.pink);
        SubscribeEvents();

        currentPlayer = GetPlayer(draftPhaseStartPlayer);
    }

    public Player GetOtherPlayer(Player player)
    {
        if (player == bluePlayer)
            return pinkPlayer;
        return bluePlayer;
    }

    public void NextPlayer() 
    {
        currentPlayer.IncreaseRoundCounter();
        currentPlayer = GetOtherPlayer(currentPlayer);
    }

    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsBlueTurn()
    {
        return currentPlayer == bluePlayer;
    }

    public bool IsCurrentPlayer(string name)
    {
        if (name.ToLower().Contains(PlayerType.blue.ToString()) && IsBlueTurn())
            return true;
        if (name.ToLower().Contains(PlayerType.pink.ToString()) && !IsBlueTurn())
            return true;
        return false;
    }

    public Player GetPlayer(PlayerType playerType)
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