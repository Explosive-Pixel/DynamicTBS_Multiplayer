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
            HandlePlacement();
        }
        else 
        {
            HandleAction();
        }
    }

    private void HandlePlacement() 
    { 
        
    }

    private void HandleAction() 
    {
        
    }

    private Character GetCharacterByPosition(Vector3 position)
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
        }

        RaycastHit hit;
        Ray ray = currentCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject gameObject = hit.transform.gameObject;
            if (gameObject && gameObject.name.Contains("Char"))
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