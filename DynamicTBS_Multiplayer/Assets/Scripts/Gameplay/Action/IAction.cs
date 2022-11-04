using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    List<GameObject> ActionDestinations { get; }
    void CreateActionDestinations(Character character);

    List<GameObject> GetActionDestinationPositions() { return ActionDestinations; }

    void ExecuteAction(GameObject actionDestination);

    void AbortAction();
}
