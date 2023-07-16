using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteadyStandPA : MonoBehaviour, IPassiveAbility
{
    public PassiveAbilityType AbilityType { get { return PassiveAbilityType.STEADY_STAND; } }

    private Character owner;

    private void Awake()
    {
        owner = gameObject.GetComponent<Character>();
    }

    public void Apply()
    {
        foreach (Tile tile in Board.Tiles)
        {
            var defaultIsChangeable = tile.isChangeable;
            tile.isChangeable = () =>
            {
                if (tile.IsOccupied() && tile.CurrentInhabitant == owner && !IsDisabled())
                {
                    return false;
                }
                return defaultIsChangeable();
            };
        }
    }

    public bool IsDisabled()
    {
        return owner.IsStunned();
    }
}