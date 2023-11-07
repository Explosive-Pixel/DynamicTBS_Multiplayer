using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFactory : MonoBehaviour
{
    [SerializeField] private GameObject master_blue;
    [SerializeField] private GameObject master_pink;
    [SerializeField] private GameObject mechanic_blue;
    [SerializeField] private GameObject mechanic_pink;
    [SerializeField] private GameObject medic_blue;
    [SerializeField] private GameObject medic_pink;
    [SerializeField] private GameObject runner_blue;
    [SerializeField] private GameObject runner_pink;
    [SerializeField] private GameObject shooter_blue;
    [SerializeField] private GameObject shooter_pink;
    [SerializeField] private GameObject tank_blue;
    [SerializeField] private GameObject tank_pink;

    private static readonly Dictionary<CharacterType, Dictionary<PlayerType, GameObject>> characters = new();

    private void Awake()
    {
        if (characters.Count > 0)
            return;

        characters.Add(CharacterType.CaptainChar, new() { { PlayerType.blue, master_blue }, { PlayerType.pink, master_pink } });
        characters.Add(CharacterType.MechanicChar, new() { { PlayerType.blue, mechanic_blue }, { PlayerType.pink, mechanic_pink } });
        characters.Add(CharacterType.DocChar, new() { { PlayerType.blue, medic_blue }, { PlayerType.pink, medic_pink } });
        characters.Add(CharacterType.RunnerChar, new() { { PlayerType.blue, runner_blue }, { PlayerType.pink, runner_pink } });
        characters.Add(CharacterType.ShooterChar, new() { { PlayerType.blue, shooter_blue }, { PlayerType.pink, shooter_pink } });
        characters.Add(CharacterType.TankChar, new() { { PlayerType.blue, tank_blue }, { PlayerType.pink, tank_pink } });
    }

    public static Character CreateCharacter(CharacterType type, PlayerType side)
    {
        GameObject prefab = GetPrefab(type, side);

        if (prefab != null)
        {
            GameObject characterGO = Instantiate(prefab);

            if (characterGO != null)
            {
                Character character = characterGO.GetComponent<Character>();
                character.CharacterType = type;
                return character;
            }
        }

        return null;
    }

    public static GameObject GetPrefab(CharacterType type, PlayerType side)
    {
        if (!characters.ContainsKey(type))
            return null;

        return characters[type][side];
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

        GameObject characterPrefab = characters[characterType][PlayerType.blue];

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
