using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    #region Gameplay Config

    public const int maxActionsPerRound = 2;

    #endregion

    private static int remainingActions;

    private static Dictionary<Character, List<ActionType>> actionsPerCharacterPerTurn = new Dictionary<Character, List<ActionType>>();

    private static bool hasGameStarted;

    private void Awake()
    {
        UnsubscribeEvents();
        SubscribeEvents();
        ResetStates();
        hasGameStarted = false;
        ActionRegistry.RemoveAll();
    }

    public static int GetRemainingActions()
    {
        return remainingActions;
    }

    public static bool ActionAvailable(Character character, ActionType actionType)
    {
        if(actionsPerCharacterPerTurn.ContainsKey(character) && actionsPerCharacterPerTurn[character].Contains(actionType))
        {
            return false;
        }
        return true;
    }

    private void ResetStates() 
    {
        SetRemainingActions(maxActionsPerRound);
        actionsPerCharacterPerTurn.Clear();
    }

    private void OnActionFinished(ActionMetadata actionMetadata) 
    {
        SetRemainingActions(remainingActions - 1);
        if (remainingActions == 0)
        {
            PlayerManager.NextPlayer();
            ResetStates();
        } else if(actionMetadata.CharacterInAction != null)
        {
            if(actionsPerCharacterPerTurn.ContainsKey(actionMetadata.CharacterInAction))
            {
                actionsPerCharacterPerTurn[actionMetadata.CharacterInAction].Add(actionMetadata.ExecutedActionType);
            } else
            {
                actionsPerCharacterPerTurn.Add(actionMetadata.CharacterInAction, new List<ActionType>() { actionMetadata.ExecutedActionType });
            }

            if(!actionMetadata.ExecutingPlayer.HasAvailableAction())
            {
                SkipAction.Execute();
            }
        }
    }

    private void SetRemainingActions(int newRemainingActions)
    {
        remainingActions = newRemainingActions;
        GameplayEvents.RemainingActionsChanged();
    }

    private void OnPlayerTurnEnded(Player player)
    {
        // Check if other player can perform any action (move/attack/ActiveAbility) -> if not, player wins
        Player otherPlayer = PlayerManager.GetOtherPlayer(player);

        if (!otherPlayer.HasAvailableAction())
        {
            Debug.Log("Player " + otherPlayer.GetPlayerType() + " lost because player can not perform any action this turn.");
            GameplayEvents.GameIsOver(player.GetPlayerType(), GameOverCondition.NO_AVAILABLE_ACTION);
        }
    }

    private void AbortTurn()
    {
        int actionsToSkip = remainingActions;
        Debug.Log("Skipping " + remainingActions + " actions");
        while(actionsToSkip > 0)
        {
            SkipAction.Execute();
            actionsToSkip--;
        }
    }

    public static bool HasGameStarted()
    {
        return hasGameStarted;
    }

    private void SubscribeToGameplayEvents()
    {
        GameplayEvents.OnFinishAction += OnActionFinished;
        GameplayEvents.OnPlayerTurnEnded += OnPlayerTurnEnded;
        GameplayEvents.OnPlayerTurnAborted += AbortTurn;
        hasGameStarted = true;
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += SubscribeToGameplayEvents;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= SubscribeToGameplayEvents;
        GameplayEvents.OnFinishAction -= OnActionFinished;
        GameplayEvents.OnPlayerTurnEnded -= OnPlayerTurnEnded;
        GameplayEvents.OnPlayerTurnAborted -= AbortTurn;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}