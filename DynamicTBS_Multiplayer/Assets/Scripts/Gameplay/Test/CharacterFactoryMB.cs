using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFactoryMB : MonoBehaviour
{
    [SerializeField] private GameObject master_blue;
    [SerializeField] private GameObject master_pink;

    private GameObject Master(PlayerType side) { return side == PlayerType.blue ? master_blue : master_pink; }

    public GameObject CreateCharacter(CharacterType type, PlayerType side)
    {
        switch(type)
        {
            case CharacterType.MasterChar:
                return Instantiate(Master(side));
        }

        return null;
    }
}
