using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHandler : MonoBehaviour
{
    private static readonly List<Character> characters = new List<Character>();

    // Cache to find characters fast, based on their gameobject
    private static readonly Dictionary<GameObject, Character> charactersByGameObject = new Dictionary<GameObject, Character>();

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

    public static Character GetCharacterByPosition(Vector3 position)
    {
        GameObject gameObject = UIUtils.FindGameObjectByPosition(charactersByGameObject.Keys.ToList(), position);
        Debug.Log("found gameObject " + gameObject.name);
        if (gameObject && charactersByGameObject.ContainsKey(gameObject))
        {
            Debug.Log("found character");
            return charactersByGameObject[gameObject];
        }

        return null;
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

    private void SetActiveAbilityOnCooldown(Character character, ActionType actionType, Vector3 characterInitialPosition, Vector3? actionDestinationPosition)
    {
        if(actionType == ActionType.ActiveAbility)
        {
            character.SetActiveAbilityOnCooldown();
        }
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
        GameplayEvents.OnFinishAction += SetActiveAbilityOnCooldown;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnCharacterCreated -= AddCharacterToList;
        DraftEvents.OnEndDraft -= DeliverCharacterList;
        GameplayEvents.OnFinishAction -= SetActiveAbilityOnCooldown;
    }

    #endregion
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}