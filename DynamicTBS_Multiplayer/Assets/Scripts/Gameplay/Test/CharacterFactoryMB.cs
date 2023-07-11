using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFactoryMB : MonoBehaviour
{
    [SerializeField] private GameObject master_blue;
    [SerializeField] private GameObject master_pink;

    [SerializeField] private GameObject runner_blue;
    [SerializeField] private GameObject runner_pink;

    private static readonly Dictionary<CharacterType, Dictionary<PlayerType, GameObject>> characters = new();

    private void Awake()
    {
        characters.Add(CharacterType.MasterChar, new() { { PlayerType.blue, master_blue }, { PlayerType.pink, master_pink } });
        characters.Add(CharacterType.RunnerChar, new() { { PlayerType.blue, runner_blue }, { PlayerType.pink, runner_pink } });
    }

    public static CharacterMB CreateCharacter(CharacterType type, PlayerType side)
    {
        GameObject characterGO = Instantiate(characters[type][side]);

        if (characterGO != null)
            return characterGO.GetComponent<CharacterMB>();

        return null;
    }
}
