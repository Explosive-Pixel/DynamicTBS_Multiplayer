using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GameplayEvents
{
    public delegate void FinishGameplayUISetup();
    public static event FinishGameplayUISetup OnFinishGameplayUISetup;

    public delegate void FinishAction(ActionMetadata actionMetadata);
    public static event FinishAction OnFinishAction;

    public delegate void ExecuteActiveAbility(Character character);
    public static event ExecuteActiveAbility OnExecuteActiveAbility;

    public delegate void ChangeRemainingActions();
    public static event ChangeRemainingActions OnChangeRemainingActions;

    public delegate void GameOver(PlayerType? winner, GameOverCondition endGameCondition);
    public static event GameOver OnGameOver;

    public delegate void CharacterSelection(Character character);
    public static event CharacterSelection OnCharacterSelectionChange;

    public delegate void NextPlayer(PlayerType player);
    public static event NextPlayer OnPlayerTurnEnded;

    public delegate void UpdatePlayer(PlayerType currentPlayer);
    public static event UpdatePlayer OnCurrentPlayerChanged;

    public delegate void AbortTurn(PlayerType abortedTurnPlayer, int remainingActions, AbortTurnCondition abortTurnCondition);
    public static event AbortTurn OnPlayerTurnAborted;

    public delegate void ExecuteUIAction(PlayerType player, UIAction uIAction);
    public static event ExecuteUIAction OnExecuteUIAction;

    public delegate void GamePaused(bool paused);
    public static event GamePaused OnGamePause;

    public delegate void GamePausedOnline(WSMsgPauseGame msg);
    public static event GamePausedOnline OnGamePauseOnline;

    public delegate void TimerUpdate(float pinkTimeLeft, float blueTimeLeft, DateTime startTimestamp, GamePhase gamePhase, PlayerType currentPlayer);
    public static event TimerUpdate OnTimerUpdate;

    public delegate void TimerTimeout(GamePhase gamePhase, PlayerType currentPlayer);
    public static event TimerTimeout OnTimerTimeout;

    public static void GameplayUISetupFinished()
    {
        if (OnFinishGameplayUISetup != null)
            OnFinishGameplayUISetup();
    }

    public static void ActionFinished(ActionMetadata actionMetadata)
    {
        if (OnFinishAction != null)
            OnFinishAction(actionMetadata);
    }

    public static void StartExecuteActiveAbility(Character character)
    {
        if (OnExecuteActiveAbility != null)
            OnExecuteActiveAbility(character);
    }

    public static void RemainingActionsChanged()
    {
        if (OnChangeRemainingActions != null)
            OnChangeRemainingActions();
    }

    public static void GameIsOver(PlayerType? winner, GameOverCondition endGameCondition)
    {
        if (OnGameOver != null)
        {
            GameEvents.EndGamePhase(GamePhase.GAMEPLAY);
            OnGameOver(winner, endGameCondition);
        }
    }

    public static void ChangeCharacterSelection(Character character)
    {
        if (OnCharacterSelectionChange != null)
            OnCharacterSelectionChange(character);
    }

    public static void EndPlayerTurn(PlayerType player)
    {
        if (OnPlayerTurnEnded != null)
            OnPlayerTurnEnded(player);
    }

    public static void ChangeCurrentPlayer(PlayerType playerType)
    {
        if (OnCurrentPlayerChanged != null)
            OnCurrentPlayerChanged(playerType);
    }

    public static void AbortCurrentPlayerTurn(PlayerType abortedTurnPlayer, int remainingActions, AbortTurnCondition abortTurnCondition)
    {
        if (OnPlayerTurnAborted != null)
            OnPlayerTurnAborted(abortedTurnPlayer, remainingActions, abortTurnCondition);
    }

    public static void UIActionExecuted(PlayerType player, UIAction uIAction)
    {
        if (OnExecuteUIAction != null)
            OnExecuteUIAction(player, uIAction);
    }

    public static void PauseGame(bool paused)
    {
        if (OnGamePause != null)
        {
            OnGamePause(paused);
        }
    }

    public static void PauseGameOnline(WSMsgPauseGame msg)
    {
        if (OnGamePauseOnline != null)
            OnGamePauseOnline(msg);
    }

    public static void UpdateTimer(float pinkTimeLeft, float blueTimeLeft, DateTime startTimestamp, GamePhase gamePhase, PlayerType currentPlayer)
    {
        if (OnTimerUpdate != null)
            OnTimerUpdate(pinkTimeLeft, blueTimeLeft, startTimestamp, gamePhase, currentPlayer);
    }

    public static void TimerTimedOut(GamePhase gamePhase, PlayerType currentPlayer)
    {
        if (OnTimerTimeout != null)
            OnTimerTimeout(gamePhase, currentPlayer);
    }
}