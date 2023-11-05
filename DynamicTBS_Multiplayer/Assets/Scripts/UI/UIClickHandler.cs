using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIClickHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> activeAbilityTriggers;

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

        HandleKeyInputsAnyClient();

        if (!GameManager.IsPlayer())
            return;

        HandleKeyInputsAnyPlayer();

        if (!PlayerManager.ClientIsCurrentPlayer())
            return;

        HandleKeyInputsCurrentPlayer();
    }

    private void HandleClickCurrentPlayer(Ray position)
    {
        // First check whether click was onto an action field (like move destination or attack target)
        // If yes, execute that action
        bool actionExecuted = ActionUtils.ExecuteAction(position);

        // If not 
        if (!actionExecuted)
        {
            // Check whether click was onto a character
            // If yes, create action destinations for this character
            Character character = TrySelectCharacter(position);
            if (character != null)
                return;

            // If not
            // Check if click was onto active ability trigger
            GameObject activeAbilityTrigger = UIUtils.FindGameObjectByRay(activeAbilityTriggers, position);

            if (activeAbilityTrigger != null && currentCharacter != null)
            {
                currentCharacter.ExecuteActiveAbility();
                return;
            }

            // If not
            // Check if click was onto any other clickable UI element (like surrender button)
            GameObject clickableObject = UIUtils.FindGameObjectByRay(FindObjectsOfType<MonoBehaviour>().OfType<IClickableObject>().ToList().ConvertAll(o => ((MonoBehaviour)o).gameObject), position);
            if (clickableObject != null)
            {
                clickableObject.GetComponent<IClickableObject>().OnClick();
                return;
            }

            // Check if click is on UI Element (like surrender button)
            if (!UIUtils.IsHit())
            {
                // If not
                UnselectCharacter();
            }
        }
        else
        {
            UnselectCharacter();
        }
    }

    private void HandleClickAnyPlayer(Ray position)
    {
        Character character = TrySelectCharacter(position);
        if (character == null)
        {
            UnselectCharacter();
        }
    }

    private void HandleKeyInputsAnyClient()
    {
        HandleHotkeys();

        if (GameManager.IsPlayer() && PlayerManager.ClientIsCurrentPlayer())
            return;

        // Handle mouse click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 clickPosition = Input.mousePosition;
            HandleClickAnyPlayer(currentCamera.ScreenPointToRay(clickPosition));
        }
    }

    private void HandleKeyInputsAnyPlayer()
    {
        // Pause Game
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, GameplayManager.gameIsPaused ? UIAction.UNPAUSE_GAME : UIAction.PAUSE_GAME);
        }
    }

    private void HandleKeyInputsCurrentPlayer()
    {
        // Handle mouse click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 clickPosition = Input.mousePosition;
            HandleClickCurrentPlayer(currentCamera.ScreenPointToRay(clickPosition));
        }

        // Same function as pressing the "Use Active Ability" button.
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentCharacter != null && currentCharacter.MayPerformActiveAbility())
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

    private void HandleHotkeys()
    {
        // Unselect currently selected character.
        if (Input.GetKeyDown(KeyCode.Mouse1)) // Right mouse.
        {
            UnselectCharacter();
        }

        // Show complete movement pattern, not just legal moves.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowActionPattern(ActionType.Move);
        }

        // Show complete attack pattern, not just legal moves.
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowActionPattern(ActionType.Attack);
        }

        // Show complete active ability pattern, not just legal moves.
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (currentCharacter != null)
            {
                ActionUtils.ResetActionDestinations();
                currentCharacter.ActiveAbility.ShowActionPattern();
            }
        }

        if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Alpha3))
        {
            ActionUtils.HideAllActionPatterns();
            SelectCharacter(currentCharacter);
        }
    }

    private void ShowActionPattern(ActionType actionType)
    {
        if (currentCharacter != null)
        {
            ActionUtils.ResetActionDestinations();
            IAction action = ActionRegistry.GetActions().Find(action => action.ActionType == actionType);
            if (action != null)
            {
                action.ShowActionPattern(currentCharacter);
            }
        }
    }

    private Character TrySelectCharacter(Ray position)
    {
        List<GameObject> selectableCharacters = (GameManager.IsPlayer() ? CharacterManager.GetAllLivingCharactersOfSide(PlayerManager.ExecutingPlayer) : CharacterManager.GetAllLivingCharacters())
                .FindAll(character => character.IsClickable)
                .ConvertAll(character => character.gameObject);

        GameObject characterGameObject = UIUtils.FindGameObjectByRay(selectableCharacters, position);

        if (characterGameObject == null)
            return null;

        Character character = characterGameObject.GetComponent<Character>();
        GameplayEvents.ChangeCharacterSelection(character);

        return character;
    }

    private void SelectCharacter(Character character)
    {
        if (character != null)
        {
            HandleClickCurrentPlayer(UIUtils.DefaultRay(character.gameObject.transform.position));
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

        if (character != null)
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
