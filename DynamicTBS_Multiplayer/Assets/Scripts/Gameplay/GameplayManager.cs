using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private int maxActionsPerRound;
    [SerializeField] private int minActionsPerRound;

    private static int remainingActions;

    private static readonly Dictionary<Character, List<ActionType>> actionsPerCharacterPerTurn = new();

    private static bool hasGameStarted;
    public static bool HasGameStarted { get { return hasGameStarted; } }
    public static bool gameIsPaused;
    public static int MaxActionsPerRound;

    private void Awake()
    {
        MaxActionsPerRound = maxActionsPerRound;

        UnsubscribeEvents();
        SubscribeEvents();
        ResetStates();
        hasGameStarted = false;
        gameIsPaused = false;
        ActionRegistry.RemoveAll();
    }

    private void ToggleGameIsPaused(PlayerType player, UIAction uIAction)
    {
        if (uIAction == UIAction.PAUSE_GAME || uIAction == UIAction.UNPAUSE_GAME)
        {
            gameIsPaused = uIAction == UIAction.PAUSE_GAME;
            GameplayEvents.PauseGame(gameIsPaused);
        }
    }

    public static int GetRemainingActions()
    {
        return remainingActions;
    }

    public static bool ActionAvailable(Character character, ActionType actionType)
    {
        if (actionsPerCharacterPerTurn.ContainsKey(character) && actionsPerCharacterPerTurn[character].Contains(actionType))
        {
            return false;
        }
        return true;
    }

    private void OnActionFinished(ActionMetadata actionMetadata)
    {
        SetRemainingActions(remainingActions - actionMetadata.ActionCount);
        if (remainingActions <= 0)
        {
            HandleNoRemainingActions();
        }
        else if (actionMetadata.CharacterInAction != null)
        {
            if (actionsPerCharacterPerTurn.ContainsKey(actionMetadata.CharacterInAction))
            {
                actionsPerCharacterPerTurn[actionMetadata.CharacterInAction].Add(actionMetadata.ExecutedActionType);
            }
            else
            {
                actionsPerCharacterPerTurn.Add(actionMetadata.CharacterInAction, new List<ActionType>() { actionMetadata.ExecutedActionType });
            }

            // Check if player can perform any further actions (move/attack/ActiveAbility)
            CheckAvailableActions(actionMetadata.ExecutingPlayer);
        }
    }

    private void SetRemainingActions(int newRemainingActions)
    {
        remainingActions = newRemainingActions;
        GameplayEvents.RemainingActionsChanged();
    }

    private void HandleNoRemainingActions()
    {
        PlayerManager.NextPlayer();
        ResetStates();
    }

    private void CheckAvailableActions(PlayerType player)
    {
        if (GameManager.CurrentGamePhase == GamePhase.GAMEPLAY && !HasAvailableAction(player))
        {
            if (maxActionsPerRound - remainingActions <= minActionsPerRound)
            {
                GameplayEvents.AbortCurrentPlayerTurn(player, remainingActions, AbortTurnCondition.NO_AVAILABLE_ACTION);
            }
            else
            {
                GameplayEvents.GameIsOver(player, GameOverCondition.NO_AVAILABLE_ACTION);
            }
        }
    }

    private bool HasAvailableAction(PlayerType player)
    {
        List<Character> characters = CharacterManager.GetAllLivingCharactersOfSide(player);

        foreach (Character character in characters)
        {
            if (character.CanPerformAction())
            {
                return true;
            }
        }

        return false;
    }

    private void OnPlayerTurnEnded(PlayerType player)
    {
        // Check if other player can perform any action (move/attack/ActiveAbility) -> if not, player wins
        CheckAvailableActions(PlayerManager.GetOtherSide(player));
    }

    private void AbortTurn()
    {
        SetRemainingActions(0);
        HandleNoRemainingActions();
    }

    private void AbortTurn(PlayerType abortedTurnPlayer, int remainingActions, AbortTurnCondition abortTurnCondition)
    {
        AbortTurn();
    }

    private void ResetStates()
    {
        SetRemainingActions(maxActionsPerRound);
        actionsPerCharacterPerTurn.Clear();
    }

    private void SubscribeToGameplayEvents(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.GAMEPLAY)
            return;

        GameplayEvents.OnFinishAction += OnActionFinished;
        GameplayEvents.OnPlayerTurnEnded += OnPlayerTurnEnded;
        GameplayEvents.OnPlayerTurnAborted += AbortTurn;
        hasGameStarted = true;
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += SubscribeToGameplayEvents;
        GameplayEvents.OnExecuteUIAction += ToggleGameIsPaused;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= SubscribeToGameplayEvents;
        GameplayEvents.OnFinishAction -= OnActionFinished;
        GameplayEvents.OnPlayerTurnEnded -= OnPlayerTurnEnded;
        GameplayEvents.OnPlayerTurnAborted -= AbortTurn;
        GameplayEvents.OnExecuteUIAction -= ToggleGameIsPaused;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}