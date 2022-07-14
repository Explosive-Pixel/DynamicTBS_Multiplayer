using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
    private List<Character> characters = new List<Character>();
    // Cache to find characters fast, based on their gameobject
    private Dictionary<GameObject, Character> charactersByGameObject = new Dictionary<GameObject, Character>();

    private Camera currentCamera;
    private GameManager gameManager;
    private PlayerManager playerManager;

    private Character currentlySelectedChar;

    private bool isListeningToClicks;

    private void Awake()
    {
        SubscribeEvents();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        isListeningToClicks = false;
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
    }

    private void HandlePlacement(Character character)
    {
        if (character.GetSide() == playerManager.GetCurrentPlayer())
        {
            PlacementEvents.SelectCharacterForPlacement(character);
            isListeningToClicks = false;
        }   
    }

    private void MoveCharacter(Vector3 position)
    {
        Vector3 oldPosition = currentlySelectedChar.GetCharacterGameObject().transform.position;
        currentlySelectedChar.GetCharacterGameObject().transform.position = position;
        UIEvents.MoveOver(oldPosition, currentlySelectedChar);
        if (!gameManager.HasGameStarted())
        {
            PlacementEvents.AdvancePlacementOrder();
        }
        else 
        {
            GameplayEvents.MoveFinished();
        }
        
        currentlySelectedChar = null;
        isListeningToClicks = true;
    }

    private void HandleAction(Character character) 
    {
        if (character.GetSide() == playerManager.GetCurrentPlayer())
        {
            isListeningToClicks = false;
            UIEvents.SelectCharacterForMove(character);
        }
        else 
        { 
            // character got attacked
        }
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

    private void DeliverCharacterList()
    {
        DraftEvents.DeliverCharacterList(characters);
    }

    private void ListenToClicks()
    {
        isListeningToClicks = true;
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        DraftEvents.OnCharacterCreated += AddCharacterToList;
        DraftEvents.OnEndDraft += ListenToClicks;
        DraftEvents.OnEndDraft += DeliverCharacterList;
        UIEvents.OnPassMoveDestination += MoveCharacter;
        UIEvents.OnInformNoMoveDestinationsAvailable += ListenToClicks;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnCharacterCreated -= AddCharacterToList;
        DraftEvents.OnEndDraft -= ListenToClicks;
        DraftEvents.OnEndDraft -= DeliverCharacterList;
        UIEvents.OnPassMoveDestination -= MoveCharacter;
        UIEvents.OnInformNoMoveDestinationsAvailable -= ListenToClicks;
    }

    #endregion
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}