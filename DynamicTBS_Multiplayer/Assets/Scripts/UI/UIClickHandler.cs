using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIClickHandler : MonoBehaviour
{
    private Camera currentCamera;

    private static Character currentCharacter = null;
    public static Character CurrentCharacter { get { return currentCharacter; } }

    private bool activeAbilityExecutionStarted = false;

    private void Awake()
    {
        SubscribeEvents();
        activeAbilityExecutionStarted = false;
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        if (GameplayManager.gameIsPaused)
        {
            HandleKeyInputsWhilePaused();
            return;
        }

        // Handle mouse click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 clickPosition = Input.mousePosition;
            HandleClick(currentCamera.ScreenPointToRay(clickPosition));
            return;
        }

        HandleKeyInputsAnyClient();

        if (!GameManager.IsPlayer())
            return;

        HandleKeyInputsAnyPlayer();

        if (!PlayerManager.ClientIsCurrentPlayer())
            return;

        HandleKeyInputsCurrentPlayer();
    }

    private void HandleClick(Ray position)
    {
        // Only current player may execute actions
        if (PlayerManager.ClientIsCurrentPlayer())
        {
            // Check whether click was onto an action field (like move destination or attack target)
            // If yes, execute that action
            if (ActionUtils.ExecuteAction(position))
            {
                UnselectCharacter();
                return;
            }
        }

        // Check whether click was onto a character
        // If yes, create action destinations for this character
        if (TrySelectCharacter(position) != null)
            return;

        // If not
        // Check if click was onto any other clickable UI element (like AA icon or surrender button)
        if (ClickClickableObject(position))
            return;

        // Check if click is on canvas UI Element
        if (UIUtils.IsHit())
            return;

        UnselectCharacter();
    }

    private void HandleKeyInputsAnyClient()
    {
        // Unselect currently selected character.
        if (Input.GetKeyDown(KeyCode.Mouse1)) // Right mouse.
        {
            UnselectCharacter();
        }

        if (GameManager.CurrentGamePhase != GamePhase.GAMEPLAY || currentCharacter == null)
            return;

        // Show complete movement pattern, not just legal moves.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActionUtils.ResetActionDestinations();
            ShowActionPattern(ActionType.Move);
        }

        // Show complete attack pattern, not just legal moves.
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActionUtils.ResetActionDestinations();
            ShowActionPattern(ActionType.Attack);
        }

        // Show complete active ability pattern, not just legal moves.
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ShowActiveAbilityPattern();
        }

        if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Alpha3))
        {
            HideActionPattern();
        }
    }

    public void ShowMovePattern()
    {
        ShowActionPattern(ActionType.Move);
    }

    public void ShowAttackPattern()
    {
        ShowActionPattern(ActionType.Attack);
    }

    public void ShowActiveAbilityPattern()
    {
        ShowActionPattern(ActionType.ActiveAbility);
    }

    private void ShowActionPattern(ActionType actionType)
    {
        if (GameManager.CurrentGamePhase != GamePhase.GAMEPLAY)
            return;

        ActionUtils.ResetActionDestinations();

        if (actionType == ActionType.ActiveAbility)
            currentCharacter.ActiveAbility.ShowActionPattern();
        else
        {
            IAction action = ActionRegistry.GetActions().Find(action => action.ActionType == actionType);
            if (action != null)
            {
                action.ShowActionPattern(currentCharacter);
            }
        }
    }

    public void HideActionPattern()
    {
        ActionUtils.HideAllActionPatterns();
        SelectCharacter(currentCharacter);
    }

    private void HandleKeyInputsAnyPlayer()
    {
        HandlePause();

        if (GameManager.CurrentGamePhase != GamePhase.GAMEPLAY)
            return;

        // Same function as pressing the Active Ability icon.
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentCharacter != null && currentCharacter.Side == PlayerManager.ExecutingPlayer && currentCharacter.MayPerformActiveAbility())
            {
                ActionUtils.ResetActionDestinations();
                if (activeAbilityExecutionStarted)
                {
                    Character character = currentCharacter;
                    UnselectCharacter();
                    SelectCharacter(character);
                }
                else
                {
                    currentCharacter.ExecuteActiveAbility();
                }
            }
        }
    }

    private void HandleKeyInputsCurrentPlayer()
    {

    }

    private void HandleKeyInputsWhilePaused()
    {
        if (!GameManager.IsPlayer())
            return;

        HandlePause();
    }

    private void HandlePause()
    {
        // Pause/Unpause Game
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GameManager.GameType == GameType.ONLINE)
                GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, GameplayManager.gameIsPaused ? UIAction.UNPAUSE_GAME : UIAction.PAUSE_GAME);
            else
                GameplayEvents.PauseGame(!GameplayManager.gameIsPaused);
        }
    }

    private Character TrySelectCharacter(Ray position)
    {
        List<GameObject> selectableCharacters = CharacterManager.GetAllLivingCharacters()
                .FindAll(character => character.IsClickable)
                .ConvertAll(character => character.gameObject);

        GameObject characterGameObject = UIUtils.FindGameObjectByRay(selectableCharacters, position);

        if (characterGameObject == null)
            return null;

        Character character = characterGameObject.GetComponent<Character>();
        GameplayEvents.ChangeCharacterSelection(character);

        return character;
    }

    private bool ClickClickableObject(Ray position)
    {
        var clickableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IClickableObject>().ToList().ConvertAll(o => ((MonoBehaviour)o).gameObject);
        GameObject clickableObject = UIUtils.FindGameObjectByRay(clickableObjects, position);
        if (clickableObject != null)
        {
            IClickableObject co = clickableObject.GetComponent<IClickableObject>();
            if (co.ClickPermission == ClickPermission.ANY_CLIENT || (GameManager.IsPlayer() && co.ClickPermission == ClickPermission.ANY_PLAYER) || (PlayerManager.ClientIsCurrentPlayer() && co.ClickPermission == ClickPermission.CURRENT_PLAYER))
            {
                co.OnClick();
                return true;
            }
        }

        return false;
    }

    private void SelectCharacter(Character character)
    {
        if (character != null)
        {
            HandleClick(UIUtils.DefaultRay(character.gameObject.transform.position));
        }
    }

    private void SetActiveAbilityStarted(Character character)
    {
        activeAbilityExecutionStarted = true;
    }

    private void ChangeCharacterSelection(Character character)
    {
        ActionUtils.ResetActionDestinations();
        activeAbilityExecutionStarted = false;
        currentCharacter = character;

        if (character != null && (!GameManager.IsPlayer() || character.Side == PlayerManager.ExecutingPlayer))
            ActionUtils.InstantiateAllActionPositions(character);
    }

    private void UnselectCharacter()
    {
        GameplayEvents.ChangeCharacterSelection(null);
    }

    private void UnselectCharacter(ActionMetadata actionMetadata)
    {
        UnselectCharacter();
    }

    private void UnselectCharacter(PlayerType abortedTurnPlayer, int remainingActions, AbortTurnCondition abortTurnCondition)
    {
        if (abortedTurnPlayer == PlayerManager.ExecutingPlayer)
            UnselectCharacter();
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameplayEvents.OnCharacterSelectionChange += ChangeCharacterSelection;
        GameplayEvents.OnExecuteActiveAbility += SetActiveAbilityStarted;
        GameplayEvents.OnFinishAction += UnselectCharacter;
        GameplayEvents.OnPlayerTurnAborted += UnselectCharacter;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnCharacterSelectionChange -= ChangeCharacterSelection;
        GameplayEvents.OnExecuteActiveAbility -= SetActiveAbilityStarted;
        GameplayEvents.OnPlayerTurnAborted -= UnselectCharacter;
        GameplayEvents.OnFinishAction -= UnselectCharacter;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
