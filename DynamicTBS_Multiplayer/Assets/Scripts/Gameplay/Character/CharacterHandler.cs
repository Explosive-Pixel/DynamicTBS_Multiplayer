using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHandler : MonoBehaviour
{
    private static readonly List<Character> characters = new List<Character>();

    public static List<Character> Characters { get { return characters; } }

    private void Awake()
    {
        SubscribeEvents();
        characters.Clear();
    }

    public static List<Character> GetAllLivingCharacters()
    {
        return characters.FindAll(character => character.GetCharacterGameObject() != null);
    }

    public static Character GetCharacterByGameObject(GameObject characterGameObject)
    {
        return characters.Find(character => character.GetCharacterGameObject() == characterGameObject);
    }

    public static Character GetCharacterByPosition(Vector3 position)
    {
        List<GameObject> characterGameObjects = GetAllLivingCharacters().ConvertAll(character => character.GetCharacterGameObject());
        GameObject gameObject = UIUtils.FindGameObjectByPosition(characterGameObjects, position);
        if (gameObject && characterGameObjects.Contains(gameObject))
        {
            return GetCharacterByGameObject(gameObject);
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
    }

    private void SetActiveAbilityOnCooldown(ActionMetadata actionMetadata)
    {
        if(actionMetadata.ExecutedActionType == ActionType.ActiveAbility)
        {
            actionMetadata.CharacterInAction.SetActiveAbilityOnCooldown();
        }
    }

    private void UpdateCharactersAfterCharacterDeath(Character character, Vector3 position)
    {
        characters.Remove(character);
    }

    private void PrepareCharacters(GamePhase gamePhase)
    {
        if(gamePhase == GamePhase.GAMEPLAY)
            characters.ForEach(c => c.isClickable = true);
    }

    private void HighlightCharacter(Character character)
    {
        GetAllLivingCharacters().ForEach(c => c.Highlight(false));

        if(character != null)
        {
            character.Highlight(true);
        }
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        DraftEvents.OnCharacterCreated += AddCharacterToList;
        GameEvents.OnGamePhaseStart += PrepareCharacters;
        GameplayEvents.OnFinishAction += SetActiveAbilityOnCooldown;
        GameplayEvents.OnCharacterSelectionChange += HighlightCharacter;
        CharacterEvents.OnCharacterDeath += UpdateCharactersAfterCharacterDeath;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnCharacterCreated -= AddCharacterToList;
        GameEvents.OnGamePhaseStart -= PrepareCharacters;
        GameplayEvents.OnFinishAction -= SetActiveAbilityOnCooldown;
        GameplayEvents.OnCharacterSelectionChange -= HighlightCharacter;
        CharacterEvents.OnCharacterDeath -= UpdateCharactersAfterCharacterDeath;
    }

    #endregion
    
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}