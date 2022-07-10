using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
    public static Character CreateCharacter(CharacterType type, Player side) 
    {
        Character character = null;
        switch (type) 
        {
            case CharacterType.MasterChar:
                character = new MasterChar(side);
                break;
            case CharacterType.TankChar:
                character = new TankChar(side);
                break;
            case CharacterType.ShooterChar:
                character = new ShooterChar(side);
                break;
            case CharacterType.RunnerChar:
                character = new RunnerChar(side);
                break;
            case CharacterType.MechanicChar:
                character = new MechanicChar(side);
                break;
            case CharacterType.MedicChar:
                character = new MedicChar(side);
                break;
        }

        return character;
    }

    public static Character CreateCharacter(string type, Player side)
    {
        if (type.Contains("_"))
        {
            type = type.Split("_")[0];
        }

        Enum.TryParse(type, out CharacterType characterType);
        return CreateCharacter(characterType, side);
    }
}