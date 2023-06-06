using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStateType
{
    STUNNED
}

public static class CharacterStateFactory
{
    public static State Create(CharacterStateType characterStateType, GameObject parent)
    {
        switch (characterStateType)
        {
            case CharacterStateType.STUNNED:
                return new StunnedState(parent);
        }

        return null;
    }
}
