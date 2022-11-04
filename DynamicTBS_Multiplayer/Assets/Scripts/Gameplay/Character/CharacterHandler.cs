using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHandler : MonoBehaviour
{
    private static List<Character> characters = new List<Character>();
    // Cache to find characters fast, based on their gameobject
    private static Dictionary<GameObject, Character> charactersByGameObject = new Dictionary<GameObject, Character>();

    private void Awake()
    {
        SubscribeEvents();
    }

    public static List<Character> GetAllLivingCharacters()
    {
        return characters.FindAll(character => character.GetCharacterGameObject() != null);
    }

    public static Character GetCharacterByGameObject(GameObject characterGameObject)
    {
        return charactersByGameObject[characterGameObject];
    }

    public static bool Neighbors(Character c1, Character c2, PatternType patternType)
    {
        Tile c1Tile = Board.GetTileByCharacter(c1);
        Tile c2Tile = Board.GetTileByCharacter(c2);

        if (c1Tile == null || c2Tile == null) return false;

        return Board.Neighbors(c1Tile, c2Tile, patternType);
    }

    public static bool AlliedNeighbors(Character c1, Character c2, PatternType patternType)
    {
        return c1.GetSide() == c2.GetSide() && Neighbors(c1, c2, patternType);
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

    #region EventsRegion

    private void SubscribeEvents()
    {
        DraftEvents.OnCharacterCreated += AddCharacterToList;
        DraftEvents.OnEndDraft += DeliverCharacterList;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnCharacterCreated -= AddCharacterToList;
        DraftEvents.OnEndDraft -= DeliverCharacterList;
    }

    #endregion
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}