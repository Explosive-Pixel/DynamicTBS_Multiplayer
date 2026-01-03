using System;
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

    private Action CurrentAction { get; set; } = new();

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

    public void ExecuteAction(Action actionToExecute)
    {
        if (actionToExecute.ActionSteps == null || actionToExecute.ActionSteps.Count == 0)
            return;

        if (actionToExecute.IsAction(ActionType.ActiveAbility))
        {
            actionToExecute.ActionSteps[0].CharacterInAction.ActiveAbility.Execute();
        }

        foreach (IAction action in ActionRegistry.GetActions())
        {
            if (action.ActionType == actionToExecute.ActionSteps[0].ActionType)
            {
                FinishAction(action, actionToExecute);
            }
        }
    }

    public bool ExecuteAction(IAction action, GameObject actionDestination)
    {
        ActionStep actionStep = action.BuildAction(actionDestination);
        CurrentAction.AddActionStep(actionStep);

        if (actionStep.ActionFinished)
        {
            CurrentAction.ExecutingPlayer = PlayerManager.CurrentPlayer;
            ExecuteOriginalAction(CurrentAction);

            ResetActionDestinations();

            return true;
        }

        return false;
    }

    public void ExecuteOriginalAction(Action actionToExecute)
    {
        if (actionToExecute.IsAction(ActionType.ActiveAbility) && ActionRegistry.GetActions().Find(action => action.ActionType == actionToExecute.ActionSteps[0].ActionType) == null)
        {
            actionToExecute.ActionSteps[0].CharacterInAction.ActiveAbility.Execute();
        }

        foreach (IAction action in ActionRegistry.GetActions())
        {
            if (action.ActionType == actionToExecute.ActionSteps[0].ActionType)
            {
                if (GameManager.GameType == GameType.LOCAL)
                {
                    FinishAction(action, actionToExecute);
                    return;
                }

                WSMsgPerformAction.SendPerformActionMessage(actionToExecute);
            }
        }
    }

    public void FinishAction(IAction action, Action actionToExecute)
    {
        action.ExecuteAction(actionToExecute);
        ResetActionDestinations();
    }

    public void ResetActions()
    {
        ResetActionDestinations();
    }

    public void ResetActionDestinations()
    {
        AbortAllActions();
        HideAllActionPatterns();

        if (CharacterManager.GetAllLivingCharacters().Find(character => character.IsHypnotized()) != null)
            return;

        CurrentAction = new();
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