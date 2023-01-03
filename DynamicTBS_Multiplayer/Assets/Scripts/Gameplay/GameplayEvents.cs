using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayEvents
{
    public delegate void GameplayPhase();
    public static event GameplayPhase OnGameplayPhaseStart;
    public static event GameplayPhase OnRestartGame;

    public delegate void FinishAction(ActionMetadata actionMetadata);
    public static event FinishAction OnFinishAction;

    public delegate void ChangeRemainingActions();
    public static event ChangeRemainingActions OnChangeRemainingActions;

    public delegate void GameOver(PlayerType? winner);
    public static event GameOver OnGameOver;

    public delegate void CharacterSelection(Character character);
    public static event CharacterSelection OnCharacterSelectionChange;

    public delegate void NextPlayer(Player player);
    public static event NextPlayer OnPlayerTurnEnded;

    public delegate void ExecuteUIAction(Player player, UIActionType uIActionType);
    public static event ExecuteUIAction OnExecuteUIAction;

    public static void StartGameplayPhase()
    {
        if (OnGameplayPhaseStart != null)
            OnGameplayPhaseStart();
    }

    public static void RestartGameplay()
    {
        if (OnRestartGame != null)
            OnRestartGame();
    }

    public static void ActionFinished(ActionMetadata actionMetadata)
    {
        if (OnFinishAction != null)
            OnFinishAction(actionMetadata);
    }

    public static void RemainingActionsChanged()
    {
        if (OnChangeRemainingActions != null)
            OnChangeRemainingActions();
    }

    public static void GameIsOver(PlayerType? winner)
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

    public static void UIActionExecuted(Player player, UIActionType uIActionType)
    {
        if (OnExecuteUIAction != null)
            OnExecuteUIAction(player, uIActionType);
    }
}