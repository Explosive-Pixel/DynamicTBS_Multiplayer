using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    ActionType ActionType { get; }
    List<GameObject> ActionDestinations { get; }
    CharacterMB CharacterInAction { get; }

    void ShowActionPattern(CharacterMB character);

    void HideActionPattern();

    int CountActionDestinations(CharacterMB character);

    void CreateActionDestinations(CharacterMB character);

    void ExecuteAction(GameObject actionDestination);

    void AbortAction();
}
