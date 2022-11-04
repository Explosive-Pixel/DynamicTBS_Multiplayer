using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClickHandler : MonoBehaviour
{
    private Camera currentCamera;

    private void Update()
    {
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
        // First check whether click was onto a action field (like move destination or attack target)
        // If yes, execute that action
        bool noActionExecuted = true;
        foreach (IAction action in ActionRegistry.GetActions()) {
            GameObject hit = FindGameObjectByClickPosition(action.GetActionDestinationPositions(), clickPosition);
            if (hit != null)
            { 
                action.ExecuteAction(hit);
                GameplayEvents.ActionFinished();
                noActionExecuted = false; 
            } 
            else 
                action.AbortAction();
        }

        // If no, check whether click was onto a character
        // If yes, create action destinations for this character
        if (noActionExecuted) {
            List<GameObject> charactersOfPlayer = CharacterHandler.GetAllLivingCharacters()
                .FindAll(character => character.GetSide() == PlayerManager.GetCurrentPlayer())
                .ConvertAll(character => character.GetCharacterGameObject());

            GameObject characterGameObject = FindGameObjectByClickPosition(charactersOfPlayer, clickPosition);

            if (characterGameObject == null)
            {
                // TODO: Check if click is on UI Element (like active ability button)
                // If not:
                //GameplayEvents.ChangeCharacterSelection(null);
                return;
            }

            Character character = CharacterHandler.GetCharacterByGameObject(characterGameObject);
            foreach (IAction action in ActionRegistry.GetActions())
            {
                action.CreateActionDestinations(character);
                GameplayEvents.ChangeCharacterSelection(character);
            }  
        } else
        {
            GameplayEvents.ChangeCharacterSelection(null);
        }

    }

    private GameObject FindGameObjectByClickPosition(List<GameObject> destinations, Vector3 clickPosition)
    {
        Ray ray = currentCamera.ScreenPointToRay(clickPosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if (hits != null && hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                GameObject gameObject = hit.transform.gameObject;
                if (destinations.Contains(gameObject))
                    return gameObject;
            }
        }

        return null;
    }
}
