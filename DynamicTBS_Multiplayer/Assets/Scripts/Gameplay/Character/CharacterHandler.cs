using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
    private List<Character> characters = new List<Character>();
    // Chache, um schneller Character anhand ihres GameObjects zu finden
    private Dictionary<GameObject, Character> charactersByGameObject = new Dictionary<GameObject, Character>();

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
        Character character = GetCharacterByClickPosition(Input.mousePosition);

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

    private Character GetCharacterByClickPosition(Vector3 position)
    {
        Ray ray = currentCamera.ScreenPointToRay(position);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if (hits != null && hits.Length > 0) 
        {
            foreach(RaycastHit hit in hits)
            {
                GameObject gameObject = hit.transform.gameObject;
                if (gameObject && charactersByGameObject.ContainsKey(gameObject))
                {
                    return charactersByGameObject.GetValueOrDefault(gameObject);
                }
            }
        }

        return null;
    }

    private void AddCharacterToList(Character character)
    {
        characters.Add(character);
        charactersByGameObject.Add(character.GetCharacterGameObject(), character);
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