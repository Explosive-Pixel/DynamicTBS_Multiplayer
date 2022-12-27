using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayEvents
{
    public delegate void GameplayPhase();
    public static event GameplayPhase OnGameplayPhaseStart;

    public delegate void FinishAction(Character character, ActionType actionType, Vector3 characterInitialPosition, Vector3? actionDestinationPosition);
    public static event FinishAction OnFinishAction;

    public delegate void GameOver(PlayerType winner);
    public static event GameOver OnGameOver;

    public delegate void CharacterSelection(Character character);
    public static event CharacterSelection OnCharacterSelectionChange;

    public delegate void NextPlayer(Player player);
    public static event NextPlayer OnPlayerTurnEnded;

    public static void StartGameplayPhase()
    {
        if (OnGameplayPhaseStart != null)
            OnGameplayPhaseStart();
    }

    public static void ActionFinished(Character character, ActionType actionType, Vector3 characterInitialPosition, Vector3? actionDestinationPosition)
    {
        if (OnFinishAction != null)
            OnFinishAction(character, actionType, characterInitialPosition, actionDestinationPosition);
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
}