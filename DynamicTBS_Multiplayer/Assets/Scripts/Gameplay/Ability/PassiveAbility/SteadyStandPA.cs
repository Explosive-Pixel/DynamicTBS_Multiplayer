using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteadyStandPA : MonoBehaviour, IPassiveAbility
{
    public void Apply()
    {
        foreach (TileMB tile in BoardNew.Tiles)
        {
            var defaultIsChangeable = tile.isChangeable;
            tile.isChangeable = () =>
            {
                if (tile.IsOccupied() && tile.CurrentInhabitant.PassiveAbility.GetType() == typeof(SteadyStandPA) && !tile.CurrentInhabitant.IsStunned())
                {
                    return false;
                }
                return defaultIsChangeable();
            };
        }
    }
}