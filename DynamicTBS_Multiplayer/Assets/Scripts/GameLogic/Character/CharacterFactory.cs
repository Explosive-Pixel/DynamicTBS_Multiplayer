using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
    [SerializeField] private GameObject captain;
    [SerializeField] private GameObject mechanic;
    [SerializeField] private GameObject doc;
    [SerializeField] private GameObject runner;
    [SerializeField] private GameObject shooter;
    [SerializeField] private GameObject tank;

    private static readonly Dictionary<CharacterType, GameObject> characters = new();

    private void Awake()
    {
        if (characters.Count > 0)
            return;

        characters.Add(CharacterType.CaptainChar, captain);
        characters.Add(CharacterType.MechanicChar, mechanic);
        characters.Add(CharacterType.DocChar, doc);
        characters.Add(CharacterType.RunnerChar, runner);
        characters.Add(CharacterType.ShooterChar, shooter);
        characters.Add(CharacterType.TankChar, tank);
    }

    public static Character CreateCharacter(CharacterType type, PlayerType side)
    {
        GameObject prefab = GetPrefab(type);

        if (prefab != null)
        {
            GameObject characterGO = Instantiate(prefab);

            if (characterGO != null)
            {
                Character character = characterGO.GetComponent<Character>();
                character.Init(type, side);
                return character;
            }
        }

        return null;
    }

    public static GameObject GetPrefab(CharacterType type)
    {
        if (!characters.ContainsKey(type))
            return null;

        return characters[type];
    }

    public static CharacterType GetRandomCharacterType()
    {
        int characterCount = Enum.GetNames(typeof(CharacterType)).Length;
        return (CharacterType)RandomNumberGenerator.GetInt32(2, characterCount + 1);
    }

    public static string GetPrettyName(CharacterType characterType)
    {
        if (!characters.ContainsKey(characterType))
            return null;

        GameObject characterPrefab = characters[characterType];

        if (characterPrefab != null)
        {
            if (characterPrefab.TryGetComponent(out Character character))
            {
                return character.PrettyName;
            }
        }

        return null;
    }
}
