using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFactoryMB : MonoBehaviour
{
    [SerializeField] private GameObject master_blue;
    [SerializeField] private GameObject master_pink;

    private GameObject Master(PlayerType side) { return side == PlayerType.blue ? master_blue : master_pink; }

    public CharacterMB CreateCharacter(CharacterType type, PlayerType side)
    {
        GameObject characterGO = null;

        switch (type)
        {
            case CharacterType.MasterChar:
                characterGO = Instantiate(Master(side));
                break;
        }

        if (characterGO != null)
            return characterGO.GetComponent<CharacterMB>();

        return null;
    }
}
