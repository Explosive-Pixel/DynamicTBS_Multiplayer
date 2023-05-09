using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    MasterChar = 1,
    MechanicChar = 2,
    MedicChar = 3,
    RunnerChar = 4,
    ShooterChar = 5,
    TankChar = 6
}

static class CharacterTypeMethods
{
    public static string Name(this CharacterType characterType)
    {
        switch (characterType)
        {
            case CharacterType.MasterChar:
                return MasterChar.name;
            case CharacterType.MechanicChar:
                return MechanicChar.name;
            case CharacterType.MedicChar:
                return MedicChar.name;
            case CharacterType.RunnerChar:
                return RunnerChar.name;
            case CharacterType.ShooterChar:
                return ShooterChar.name;
            case CharacterType.TankChar:
                return TankChar.name;
            default:
                return "";
        }
    }
}