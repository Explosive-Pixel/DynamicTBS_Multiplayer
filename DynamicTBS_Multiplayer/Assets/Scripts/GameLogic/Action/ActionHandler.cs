using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : MonoBehaviour
{
    public static ActionHandler Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    private List<ActionStep> CurrentAction { get; set; } = new();

    public void InstantiateAllActionPositions(Character character)
    {
        ResetActionDestinations();
        foreach (IAction action in ActionRegistry.GetActions())
        {
            if (GameplayManager.ActionAvailable(character, action.ActionType) && !character.isDisabled())
            {
                action.CreateActionDestinations(character);
            }
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        foreach (IAction action in ActionRegistry.GetActions())
        {
            if (action.ActionDestinations.Contains(actionDestination))
            {
                ExecuteAction(action, actionDestination);
            }
        }
    }

    public bool ExecuteAction(Vector3 position)
    {
        foreach (IAction action in ActionRegistry.GetActions())
        {
            GameObject hit = UIUtils.FindGameObjectByPosition(action.ActionDestinations, position);
            if (hit != null)
            {
                return ExecuteAction(action, hit);
            }
            else
                action.AbortAction();
        }

        return false;
    }


    public bool ExecuteAction(IAction action, GameObject actionDestination)
    {
        ActionStep actionStep = action.ExecuteAction(actionDestination);

        if (CurrentAction.Count > 0 && CurrentAction[^1].ActionFinished)
        {
            CurrentAction = new();
        }
        CurrentAction.Add(actionStep);

        if (actionStep.ActionFinished)
        {
            GameplayEvents.ActionFinished(new Action
            {
                ExecutingPlayer = PlayerManager.CurrentPlayer,
                ActionSteps = CurrentAction
            });

            ResetActionDestinations();

            return true;
        }

        return false;
    }

    public void ResetActions()
    {
        ResetActionDestinations();
        CurrentAction = new();
    }

    public void ResetActionDestinations()
    {
        AbortAllActions();
        HideAllActionPatterns();
    }

    public void AbortAllActions()
    {
        foreach (IAction action in ActionRegistry.GetActions())
        {
            action.AbortAction();
        }
    }

    public void HideAllActionPatterns()
    {
        ActionRegistry.HideAllActionPatterns();
    }
}