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
    public static bool UIInteractionAllowed
    {
        get
        {
            return SceneChangeManager.Instance.CurrentScene == Scene.TUTORIAL
                || (GameManager.CurrentGamePhase != GamePhase.NONE
                && !gameIsPaused &&
                (GameManager.GameType == GameType.LOCAL
                || (!Client.IsLoadingGame && Client.InLobby)));
        }
    }
    public static bool UIPlayerActionAllowed
    {
        get
        {
            return UIInteractionAllowed && GameManager.IsPlayer();
        }
    }
    public static bool IsLoadingGame { get { return GameManager.GameType == GameType.ONLINE && Client.IsLoadingGame; } }
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

    private void ToggleGameIsPaused(bool paused)
    {
        gameIsPaused = paused;
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

    private void OnActionFinished(Action action)
    {
        SetRemainingActions(remainingActions - 1);
        if (remainingActions <= 0)
        {
            HandleNoRemainingActions();
        }
        else if (action.ActionSteps != null)
        {
            action.ActionSteps.ForEach(actionStep =>
            {
                if (!actionsPerCharacterPerTurn.ContainsKey(actionStep.CharacterInAction))
                {
                    actionsPerCharacterPerTurn.Add(actionStep.CharacterInAction, new());
                }
                actionsPerCharacterPerTurn[actionStep.CharacterInAction].Add(actionStep.ActionType);
            });

            // Check if player can perform any further actions (move/attack/ActiveAbility)
            CheckAvailableActions(action.ExecutingPlayer);
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

        foreach (IPlayerAction playerAction in PlayerActionRegistry.GetActions())
        {
            if (playerAction.IsActionAvailable(player))
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

        WSMsgUpdateServer.SendUpdateServerMessage(GamePhase.GAMEPLAY);
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += SubscribeToGameplayEvents;
        GameplayEvents.OnGamePause += ToggleGameIsPaused;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= SubscribeToGameplayEvents;
        GameplayEvents.OnFinishAction -= OnActionFinished;
        GameplayEvents.OnPlayerTurnEnded -= OnPlayerTurnEnded;
        GameplayEvents.OnPlayerTurnAborted -= AbortTurn;
        GameplayEvents.OnGamePause -= ToggleGameIsPaused;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}