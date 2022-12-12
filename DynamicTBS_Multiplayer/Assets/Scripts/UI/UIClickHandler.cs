using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClickHandler : MonoBehaviour
{
    private Camera currentCamera;
    public GameObject cardhandler;
    public GameObject uibutton;

    private void Start()
    {
        GameplayEvents.OnGameplayPhaseStart += onStartGameplayPhase;
    }

    private void Update()
    {
        // In multiplayer mode only listen to clicks of current player
        if (GameManager.gameType == GameType.multiplayer && Client.Instance.side != PlayerManager.GetCurrentPlayer().GetPlayerType())
            return;

        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 clickPosition = Input.mousePosition;
            HandleClick(clickPosition);
        }
    }

    private void HandleClick(Vector3 clickPosition)
    {
        // First check whether click was onto an action field (like move destination or attack target)
        // If yes, execute that action
        bool actionExecuted = ActionUtils.ExecuteAction(currentCamera.ScreenPointToRay(clickPosition));

        // If not, check whether click was onto a character
        // If yes, create action destinations for this character
        if (!actionExecuted) {
            List<GameObject> charactersOfPlayer = CharacterHandler.GetAllLivingCharacters()
                .FindAll(character => character.GetSide() == PlayerManager.GetCurrentPlayer())
                .ConvertAll(character => character.GetCharacterGameObject());

            GameObject characterGameObject = UIUtils.FindGameObjectByRay(charactersOfPlayer, currentCamera.ScreenPointToRay(clickPosition));

            if (characterGameObject == null)
            {
                // TODO: Check if click is on UI Element (like active ability button)
                // If not:
                //GameplayEvents.ChangeCharacterSelection(null);
                return;
            }

            Character character = CharacterHandler.GetCharacterByGameObject(characterGameObject);
            ActionUtils.InstantiateAllActionPositions(character);
            //TODO: in gameManager verlegen
            /*
            if (characterGameObject != null && hasStarted)
            {
                cardhandler.GetComponent<cardhandleScript>().setActive(character.GetCharacterType());
                uibutton.SetActive(true);
            }
            */

            
            GameplayEvents.ChangeCharacterSelection(character);
        } else
        {
            GameplayEvents.ChangeCharacterSelection(null);
        }
    }
}
