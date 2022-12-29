using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    ActionType ActionType { get; }
    List<GameObject> ActionDestinations { get; }
    Character CharacterInAction { get; }

    int CountActionDestinations(Character character);

    void CreateActionDestinations(Character character);
   
    void ExecuteAction(GameObject actionDestination);

    void AbortAction();
}
