using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    // Implement the following win/loss/draw-conditions...
    // (DONE) WIN/LOSS: A side wins, if...
    //           - the master unit is first to activate its active ability on the goal square.
    //           - the opposing master unit is killed while the own master unit is still alive.
    //           - the opposing side has no more legal moves, but 2 actions in their turn.
    // (TODO) DRAW: A game is drawn, if...
    //           - no mechanic units are alive on either side...
    //           & the goal square can't be reached...
    //           & both master units can't be killed.

    #region Gameplay Config

    private const int maxActionsPerRound = 2;

    #endregion

    private static int remainingActions;

    private static Dictionary<Character, List<ActionType>> actionsPerCharacterPerTurn = new Dictionary<Character, List<ActionType>>();

    private void Awake()
    {
        SubscribeEvents();
        ResetStates();
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
        remainingActions = maxActionsPerRound;
        actionsPerCharacterPerTurn.Clear();
    }

    private void OnActionFinished(Character character, ActionType actionType, Vector3 characterInitialPosition, Vector3? actionDestinationPosition) 
    {
        remainingActions--;
        if (remainingActions == 0)
        {
            PlayerManager.NextPlayer();
            ResetStates();
        } else
        {
            if(actionsPerCharacterPerTurn.ContainsKey(character))
            {
                actionsPerCharacterPerTurn[character].Add(actionType);
            } else
            {
                actionsPerCharacterPerTurn.Add(character, new List<ActionType>() { actionType });
            }
        }
    }

    private void OnPlayerTurnEnded(Player player)
    {
        // Check if other player can perform any action (move/attack/ActiveAbility) -> if not, player wins
        Player otherPlayer = PlayerManager.GetOtherPlayer(player);

        List<Character> charactersOfOtherPlayer = CharacterHandler.GetAllLivingCharacters()
               .FindAll(character => character.GetSide() == otherPlayer);

        foreach(Character character in charactersOfOtherPlayer)
        {
            if(character.CanPerformAction())
            {
                return;
            }
        }

        Debug.Log("Player " + otherPlayer.GetPlayerType() + " lost because player can not perform any action this round.");
        GameplayEvents.GameIsOver(player.GetPlayerType());
    }

    private void SubscribeToGameplayEvents()
    {
        GameplayEvents.OnFinishAction += OnActionFinished;
        GameplayEvents.OnPlayerTurnEnded += OnPlayerTurnEnded;
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
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}