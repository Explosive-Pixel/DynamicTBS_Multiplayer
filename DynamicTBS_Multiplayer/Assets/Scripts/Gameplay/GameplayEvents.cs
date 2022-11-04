using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayEvents
{
    public delegate void GameplayPhase();
    public static event GameplayPhase OnGameplayPhaseStart;

    public delegate void FinishAction();
    public static event FinishAction OnFinishAction;

    public delegate void GameOver(PlayerType winner);
    public static event GameOver OnGameOver;

    public delegate void CharacterSelection(Character character);
    public static event CharacterSelection OnCharacterSelectionChange;

    public delegate void NextPlayer(Player player);
    public static event NextPlayer OnPlayerTurnEnded;

    public delegate void ChangeTile(Tile oldTile, Tile newTile);
    public static event ChangeTile OnTileChanged;

    public static void StartGameplayPhase()
    {
        if (OnGameplayPhaseStart != null)
            OnGameplayPhaseStart();
    }

    public static void ActionFinished()
    {
        if (OnFinishAction != null)
            OnFinishAction();
    }

    public static void GameIsOver(PlayerType winner)
    {
        if (OnGameOver != null) 
        {
            OnGameOver(winner);
        }
    }

    public static void ChangeCharacterSelection(Character character)
    {
        if (OnCharacterSelectionChange != null)
            OnCharacterSelectionChange(character);
    }

    public static void EndPlayerTurn(Player player)
    {
        if (OnPlayerTurnEnded != null)
            OnPlayerTurnEnded(player);
    }

    public static void TileHasChanged(Tile oldTile, Tile newTile)
    {
        if (OnTileChanged != null)
            OnTileChanged(oldTile, newTile);
    }
}