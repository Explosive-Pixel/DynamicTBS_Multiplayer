using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHandler : MonoBehaviour
{
    private List<Character> characters = new List<Character>();
    // Cache to find characters fast, based on their gameobject
    private Dictionary<GameObject, Character> charactersByGameObject = new Dictionary<GameObject, Character>();

    private Camera currentCamera;
    private GameManager gameManager;
    private PlayerManager playerManager;
    private Board board;

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

    public Character GetCurrentlySelectedCharacter()
    {
        return currentlySelectedChar;
    }

    public void InformAboutGameOver(PlayerType winner)
    {
        GameplayEvents.GameIsOver(winner);
    }

    private void LoadBoard()
    {
        board = GameObject.Find("GameplayCanvas").GetComponent<Board>();
    }

    private void HandleClick()
    {
        Character character = GetCharacterByPosition(Input.mousePosition, true);

        if (character == null)
        {
            UpdateCurrentlySelectedCharacter(null);
            return;
        }

        if (character.GetSide() == playerManager.GetCurrentPlayer())
        {
            Debug.Log("Active ability cooldown of character " + character + " is " + character.GetActiveAbilityCooldown());
            UpdateCurrentlySelectedCharacter(character);

            if (!gameManager.HasGameStarted())
            {
                HandlePlacement(character);
            }
            else
            {
                HandleAction(character);
            }
        }
    }

    private void HandlePlacement(Character character)
    {
        PlacementEvents.SelectCharacterForPlacement(character);
        isListeningToClicks = false;
    }

    private void ExecuteAction(Vector3 position, UIActionType type)
    {
        switch (type)
        {
            case UIActionType.Move:
                {
                    MoveCharacter(position);
                    break;
                }
            case UIActionType.Attack:
                {
                    PerformAttack(position);
                    break;
                }
        }
    }

    private void PerformAttack(Vector3 position) 
    {
        Character characterToAttack = GetCharacterByPosition(position, false);
        characterToAttack.GetAttacked();

        GameplayEvents.ActionFinished(UIActionType.Attack);
    }


    private void MoveCharacter(Vector3 position)
    {
        Vector3 oldPosition = currentlySelectedChar.GetCharacterGameObject().transform.position;
        currentlySelectedChar.GetCharacterGameObject().transform.position = position;
        UIEvents.MoveOver(oldPosition, currentlySelectedChar);
        if (!gameManager.HasGameStarted())
        {
            PlacementEvents.AdvancePlacementOrder();
            ListenToClicks();
        }
        else 
        {
            GameplayEvents.ActionFinished(UIActionType.Move);
        }
    }

    private void HandleAction(Character character) 
    {
        isListeningToClicks = false;
        UIEvents.SelectCharacterForAction(character, UIActionType.Move);
        UIEvents.SelectCharacterForAction(character, UIActionType.Attack);
    }

    private Character GetCharacterByPosition(Vector3 position, bool isClick)
    {
        if (isClick)
        {
            Ray ray = currentCamera.ScreenPointToRay(position);

            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits != null && hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    GameObject gameObject = hit.transform.gameObject;
                    if (gameObject && charactersByGameObject.ContainsKey(gameObject))
                    {
                        return charactersByGameObject.GetValueOrDefault(gameObject);
                    }
                }
            }
        }
        else 
        {
            Tile tile = board.GetTileByPosition(position);
            if (tile != null)
                return tile.GetCurrentInhabitant();
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

    private void ActionOver(UIActionType actionType)
    {
        ListenToClicks();
        UpdateCurrentlySelectedCharacter(null);
    }

    private void ListenToClicks()
    {
        isListeningToClicks = true;
    }

    private void UpdateCurrentlySelectedCharacter(Character character)
    {
        currentlySelectedChar = character;
        GameplayEvents.ChangeCharacterSelection(character);
    }

    private void ReduceActiveAbiliyCooldowns(Player player)
    {
        foreach (Character character in characters)
        {
            if (character.GetSide().Equals(player))
            {
                character.ReduceActiveAbilityCooldown();
            }
        }
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        DraftEvents.OnCharacterCreated += AddCharacterToList;
        DraftEvents.OnEndDraft += ListenToClicks;
        DraftEvents.OnEndDraft += DeliverCharacterList;
        PlacementEvents.OnPlacementStart += LoadBoard;
        UIEvents.OnPassActionDestination += ExecuteAction;
        UIEvents.OnInformNoActionDestinationsAvailable += ListenToClicks;
        GameplayEvents.OnFinishAction += ActionOver;
        GameplayEvents.OnPlayerRoundEnded += ReduceActiveAbiliyCooldowns;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnCharacterCreated -= AddCharacterToList;
        DraftEvents.OnEndDraft -= ListenToClicks;
        DraftEvents.OnEndDraft -= DeliverCharacterList;
        PlacementEvents.OnPlacementStart -= LoadBoard;
        UIEvents.OnPassActionDestination -= ExecuteAction;
        UIEvents.OnInformNoActionDestinationsAvailable -= ListenToClicks;
        GameplayEvents.OnFinishAction -= ActionOver;
        GameplayEvents.OnPlayerRoundEnded -= ReduceActiveAbiliyCooldowns;
    }

    #endregion
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}