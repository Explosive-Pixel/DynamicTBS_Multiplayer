using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
    private List<Character> characters = new List<Character>();

    private void Awake()
    {
        DraftEvents.OnCharacterCreated += AddCharacterToList;
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