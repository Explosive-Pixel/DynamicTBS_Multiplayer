using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertPA : MonoBehaviour, IPassiveAbility
{
    public PassiveAbilityType AbilityType { get { return PassiveAbilityType.ALERT; } }

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
        return owner.isDisabled();
    }
}