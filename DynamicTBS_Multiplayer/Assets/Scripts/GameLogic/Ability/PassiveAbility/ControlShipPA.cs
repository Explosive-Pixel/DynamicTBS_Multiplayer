using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlShipPA : MonoBehaviour, IPassiveAbility
{
    public PassiveAbilityType AbilityType { get { return PassiveAbilityType.CONTROL_SHIP; } }

    private Character owner;

    private void Awake()
    {
        owner = gameObject.GetComponent<Character>();
    }

    public void Apply()
    {
        GameplayEvents.OnFinishAction += CheckIfOwnerMovedOntoGoalSquare;
    }

    public bool IsDisabled()
    {
        return false;
    }

    private void CheckIfOwnerMovedOntoGoalSquare(ActionMetadata actionMetadata)
    {
        if (GameManager.CurrentGamePhase != GamePhase.GAMEPLAY)
            return;

        if (owner.CurrentTile != null && owner.CurrentTile.TileType.Equals(TileType.GoalTile))
        {
            GameplayEvents.GameIsOver(owner.Side, GameOverCondition.CAPTAIN_TOOK_CONTROL);
        }
    }

    private void OnDestroy()
    {
        GameplayEvents.OnFinishAction -= CheckIfOwnerMovedOntoGoalSquare;
    }
}
