using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    ActionType ActionType { get; }
    List<GameObject> ActionDestinations { get; }
    Character CharacterInAction { get; }

    void ShowActionPattern(Character character);

    void HideActionPattern();

    int CountActionDestinations(Character character);

    void CreateActionDestinations(Character character);

    void ExecuteAction(GameObject actionDestination);

    void AbortAction();
}
