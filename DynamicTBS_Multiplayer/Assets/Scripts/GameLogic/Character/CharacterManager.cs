using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    public static Character SelectedCharacter { get; private set; } = null;

    private void Awake()
    {
        GetAllLivingCharacters().ForEach(character => Destroy(character.gameObject));

        GameplayEvents.OnCharacterSelectionChange += ChangeSelectedCharacter;
    }

    public static List<Character> GetAllLivingCharacters()
    {
        return FindObjectsOfType<Character>().ToList();
    }

    public static List<Character> GetAllLivingCharactersOfSide(PlayerType side)
    {
        return GetAllLivingCharacters().FindAll(character => character.Side == side);
    }

    public static Character GetCharacterByPosition(Vector3 position)
    {
        List<GameObject> characterGameObjects = GetAllLivingCharacters().ConvertAll(character => character.gameObject);
        GameObject gameObject = UIUtils.FindGameObjectByPosition(characterGameObjects, position);
        if (gameObject)
        {
            return gameObject.GetComponent<Character>();
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
        return c1.Side == c2.Side && Neighbors(c1, c2, patternType);
    }

    private void ChangeSelectedCharacter(Character character)
    {
        SelectedCharacter = character;
    }

    private void OnDestroy()
    {
        GameplayEvents.OnCharacterSelectionChange -= ChangeSelectedCharacter;
    }
}
