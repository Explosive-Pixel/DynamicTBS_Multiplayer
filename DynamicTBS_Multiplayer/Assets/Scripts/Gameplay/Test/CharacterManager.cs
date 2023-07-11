using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    public static List<CharacterMB> GetAllLivingCharacters()
    {
        return FindObjectsOfType<CharacterMB>().ToList();
    }

    public static CharacterMB GetCharacterByPosition(Vector3 position)
    {
        List<GameObject> characterGameObjects = GetAllLivingCharacters().ConvertAll(character => character.gameObject);
        GameObject gameObject = UIUtils.FindGameObjectByPosition(characterGameObjects, position);
        if (gameObject)
        {
            return gameObject.GetComponent<CharacterMB>();
        }

        return null;
    }

    public static bool Neighbors(CharacterMB c1, CharacterMB c2, PatternType patternType)
    {
        TileMB c1Tile = BoardNew.GetTileByCharacter(c1);
        TileMB c2Tile = BoardNew.GetTileByCharacter(c2);

        if (c1Tile == null || c2Tile == null) return false;

        return BoardNew.Neighbors(c1Tile, c2Tile, patternType);
    }

    public static bool AlliedNeighbors(CharacterMB c1, CharacterMB c2, PatternType patternType)
    {
        return c1.Side == c2.Side && Neighbors(c1, c2, patternType);
    }
}
