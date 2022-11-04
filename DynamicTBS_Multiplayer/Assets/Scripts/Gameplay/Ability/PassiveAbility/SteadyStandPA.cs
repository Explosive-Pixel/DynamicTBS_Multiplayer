using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteadyStandPA : IPassiveAbility
{
    private Character owner;

    public SteadyStandPA(Character character)
    {
        owner = character;
    }

    public void Apply() 
    {
        foreach (Tile tile in Board.GetAllTiles())
        {
            var defaultIsChangeable = tile.isChangeable;
            tile.isChangeable = () =>
            {
                if (tile.IsOccupied() && tile.GetCurrentInhabitant().GetPassiveAbility().GetType() == typeof(SteadyStandPA))
                {
                    return false;
                }
                return defaultIsChangeable();
            };
        }
    }
}