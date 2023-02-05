using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIClickHandler : MonoBehaviour
{
    private Camera currentCamera;

    private Character currentCharacter = null;

    private void Awake()
    {
        SubscribeEvents();
    }

    private void Update()
    {
        HandleKeyInputsAnyClient();

        if (!GameManager.IsPlayer())
            return;

        HandleKeyInputsAnyPlayer();

        if (!PlayerManager.ClientIsCurrentPlayer())
            return;

        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        HandleKeyInputsCurrentPlayer();
    }

    private void HandleClick(Ray position)
    {
        // First check whether click was onto an action field (like move destination or attack target)
        // If yes, execute that action
        bool actionExecuted = ActionUtils.ExecuteAction(position);

        // If not, check whether click was onto a character
        // If yes, create action destinations for this character
        if (!actionExecuted) {
            List<GameObject> charactersOfPlayer = CharacterHandler.GetAllLivingCharacters()
                .FindAll(character => character.isClickable && character.GetSide() == PlayerManager.GetCurrentPlayer())
                .ConvertAll(character => character.GetCharacterGameObject());

            GameObject characterGameObject = UIUtils.FindGameObjectByRay(charactersOfPlayer, position);

            if (characterGameObject == null)
            {
                // Check if click is on UI Element (like active ability button)
                if (!UIUtils.IsHit())
                {
                    // If not
                    UnselectCharacter();
                }
                return;
            }

            Character character = CharacterHandler.GetCharacterByGameObject(characterGameObject);
            GameplayEvents.ChangeCharacterSelection(character);
            ActionUtils.InstantiateAllActionPositions(character);
            
        } else
        {
            UnselectCharacter();
        }
    }

    private void HandleKeyInputsAnyClient()
    {
        // Toggle settings menu.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject settingsMenu = GameObject.Find("SettingsCanvas");
            if (settingsMenu != null)
            {
                settingsMenu.GetComponent<SettingsManager>().ToggleSettings();
            }
        }
    }

    private void HandleKeyInputsAnyPlayer()
    {
        // Pause Game
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameplayEvents.UIActionExecuted(PlayerManager.GetCurrentlyExecutingPlayer(), GameplayManager.gameIsPaused ? UIActionType.UnpauseGame : UIActionType.PauseGame);
        }
    }

    private void HandleKeyInputsCurrentPlayer()
    {
        // Handle mouse click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 clickPosition = Input.mousePosition;
            HandleClick(currentCamera.ScreenPointToRay(clickPosition));
        }

        // Unselect currently selected character.
        if (Input.GetKeyDown(KeyCode.Mouse1)) // Right mouse.
        {
            GameplayEvents.ChangeCharacterSelection(null);
        }

        // Same function as pressing the "Use Active Ability" button.
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentCharacter != null)
            {
                ActionUtils.ResetActionDestinations();
                GameObject activeAbilityButton = GameObject.Find("ActiveAbilityButton");
                if (activeAbilityButton != null)
                {
                    activeAbilityButton.GetComponent<Button>().onClick.Invoke();
                }
            }
        }

        // Show complete movement pattern, not just legal moves.
        if (Input.GetKey(KeyCode.Alpha1))
        {
            ShowActionPattern(ActionType.Move);
        }

        // Show complete attack pattern, not just legal moves.
        if (Input.GetKey(KeyCode.Alpha2))
        {
            ShowActionPattern(ActionType.Attack);
        }

        // Show complete active ability pattern, not just legal moves.
        if (Input.GetKey(KeyCode.Alpha3))
        {
            if(currentCharacter != null)
            {
                ActionUtils.ResetActionDestinations();
                currentCharacter.GetActiveAbility().ShowActionPattern();
            }
        }

        if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Alpha3))
        {
            ActionUtils.HideAllActionPatterns();
            if(currentCharacter != null)
            {
                HandleClick(UIUtils.DefaultRay(currentCharacter.GetCharacterGameObject().transform.position));
            }
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

    private void ChangeCharacterSelection(Character character)
    {
        ActionUtils.ResetActionDestinations();
        currentCharacter = character;
    }

    private void UnselectCharacter()
    {
        GameplayEvents.ChangeCharacterSelection(null);
    }

    private void UnselectCharacter(ActionMetadata actionMetadata)
    {
        UnselectCharacter();
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameplayEvents.OnCharacterSelectionChange += ChangeCharacterSelection;
        GameplayEvents.OnFinishAction += UnselectCharacter;
        GameplayEvents.OnPlayerTurnAborted += UnselectCharacter;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnCharacterSelectionChange -= ChangeCharacterSelection;
        GameplayEvents.OnPlayerTurnAborted -= UnselectCharacter;
        GameplayEvents.OnFinishAction -= UnselectCharacter;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
