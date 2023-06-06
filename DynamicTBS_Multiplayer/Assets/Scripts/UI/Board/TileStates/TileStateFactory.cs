using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileStateType
{
    ELECTRIFIED
}

public static class TileStateFactory
{
    public static State Create(TileStateType tileStateType, GameObject parent)
    {
        switch(tileStateType)
        {
            case TileStateType.ELECTRIFIED:
                return new ElectrifiedState(parent);
        }

        return null;
    }
}
