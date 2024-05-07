using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    CaptainChar = 1,
    MechanicChar = 2,
    DocChar = 3,
    RunnerChar = 4,
    ShooterChar = 5,
    TankChar = 6
}

static class CharacterTypeMethods
{
    public static string Name(this CharacterType characterType)
    {
        string name = CharacterFactory.GetPrettyName(characterType);

        if (name != null)
            return name;

        return characterType switch
        {
            CharacterType.CaptainChar => "Captain",
            CharacterType.MechanicChar => "Mechanic",
            CharacterType.DocChar => "Doc",
            CharacterType.RunnerChar => "Runner",
            CharacterType.ShooterChar => "Shooter",
            CharacterType.TankChar => "Tank",
            _ => null,
        };
    }
}