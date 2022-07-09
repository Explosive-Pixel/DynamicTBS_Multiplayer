using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
    private List<Character> characters = new List<Character>();

    private Camera currentCamera;
    private GameManager gameManager;

    private void Awake()
    {
        DraftEvents.OnCharacterCreated += AddCharacterToList;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Character character = GetCharacterByPosition(Input.mousePosition);

        if (character == null) 
        {
            return;
        }

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
        PlacementEvents.SelectCharacterForPlacement(character);
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

    private void OnDestroy()
    {
        DraftEvents.OnCharacterCreated -= AddCharacterToList;
    }
}