using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayEvents
{
    public delegate void GameplayPhase();
    public static event GameplayPhase OnRestartGame;

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

    public delegate void NextPlayer(Player player);
    public static event NextPlayer OnPlayerTurnEnded;

    public delegate void UpdatePlayer(PlayerType currentPlayer);
    public static event UpdatePlayer OnCurrentPlayerChanged;

    public delegate void AbortTurn(int remainingActions, AbortTurnCondition abortTurnCondition);
    public static event AbortTurn OnPlayerTurnAborted;

    public delegate void ExecuteUIAction(Player player, UIAction uIAction);
    public static event ExecuteUIAction OnExecuteUIAction;

    public delegate void ExecuteServerAction(ServerActionType serverActionType);
    public static event ExecuteServerAction OnExecuteServerAction;

    public delegate void GamePaused(bool paused);
    public static event GamePaused OnGamePause;

    public delegate void TimerUpdate(float pinkTimeLeft, float blueTimeLeft, int pinkDebuff, int blueDebuff);
    public static event TimerUpdate OnTimerUpdate;

    public delegate void TimerTimeout(GamePhase gamePhase, PlayerType currentPlayer, int currentPlayerTimerDebuff);
    public static event TimerTimeout OnTimerTimeout;

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
            GameManager.ChangeGamePhase(GamePhase.NONE);
            OnGameOver(winner, endGameCondition);
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

    public static void ChangeCurrentPlayer(PlayerType playerType)
    {
        if (OnCurrentPlayerChanged != null)
            OnCurrentPlayerChanged(playerType);
    }

    public static void AbortCurrentPlayerTurn(int remainingActions, AbortTurnCondition abortTurnCondition)
    {
        if (OnPlayerTurnAborted != null)
            OnPlayerTurnAborted(remainingActions, abortTurnCondition);
    }

    public static void UIActionExecuted(Player player, UIAction uIAction)
    {
        if (OnExecuteUIAction != null)
            OnExecuteUIAction(player, uIAction);
    }

    public static void ServerActionExecuted(ServerActionType serverActionType)
    {
        if (OnExecuteServerAction != null)
            OnExecuteServerAction(serverActionType);
    }

    public static void PauseGame(bool paused)
    {
        if(OnGamePause != null)
        {
            OnGamePause(paused);
        }
    }

    public static void UpdateTimer(float pinkTimeLeft, float blueTimeLeft, int pinkDebuff, int blueDebuff)
    {
        if (OnTimerUpdate != null)
            OnTimerUpdate(pinkTimeLeft, blueTimeLeft, pinkDebuff, blueDebuff);
    }

    public static void TimerTimedOut(GamePhase gamePhase, PlayerType currentPlayer, int currentPlayerTimerDebuff)
    {
        if (OnTimerTimeout != null)
            OnTimerTimeout(gamePhase, currentPlayer, currentPlayerTimerDebuff);
    }
}