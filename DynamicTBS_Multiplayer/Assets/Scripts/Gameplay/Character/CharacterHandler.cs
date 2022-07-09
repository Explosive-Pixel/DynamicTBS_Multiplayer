using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
    private List<Character> characters = new List<Character>();

    private Camera currentCamera;
    private GameManager gameManager;

    private Character currentlySelectedChar;

    private bool isListeningToClicks;

    private void Awake()
    {
        SubscribeEvents();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        isListeningToClicks = true;
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        if (!isListeningToClicks) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Character character = GetCharacterByPosition(Input.mousePosition);

        currentlySelectedChar = character;

        if (character == null) return;

        if (!gameManager.HasGameStarted())
        {
            HandlePlacement(character);
        }
        else 
        {
            HandleAction(character);
        }

        isListeningToClicks = false;
    }

    private void HandlePlacement(Character character) 
    { 
        PlacementEvents.SelectCharacterForPlacement(character);
    }

    private void MoveCharacter(Vector3 position)
    {
        Vector3 oldPosition = currentlySelectedChar.GetCharacterGameObject().transform.position;
        currentlySelectedChar.GetCharacterGameObject().transform.position = position;
        UIEvents.MoveOver(oldPosition, currentlySelectedChar);
        
        currentlySelectedChar = null;
        isListeningToClicks = true;
    }

    private void HandleAction(Character character) 
    {
        
    }

    private Character GetCharacterByPosition(Vector3 position)
    {
        RaycastHit hit;
        Ray ray = currentCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject gameObject = hit.transform.gameObject;
            if (gameObject)
            {
                return characters.Find(c => c.GetCharacterGameObject().Equals(gameObject));
            }
        }

        return null;
    }

    private void AddCharacterToList(Character character)
    {
        characters.Add(character);
    }

    #region MyRegion

    private void SubscribeEvents()
    {
        DraftEvents.OnCharacterCreated += AddCharacterToList;
        UIEvents.OnPassMoveDestination += MoveCharacter;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnCharacterCreated -= AddCharacterToList;
        UIEvents.OnPassMoveDestination -= MoveCharacter;
    }

    #endregion
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}