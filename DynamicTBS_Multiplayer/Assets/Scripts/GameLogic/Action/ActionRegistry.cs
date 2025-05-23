using System.Collections.Generic;

public class ActionRegistry
{
    private static readonly List<IAction> actions = new();
    private static readonly List<IAction> patternActions = new();

    public static void Register(IAction action)
    {
        actions.Add(action);
    }

    public static void Remove(IAction action)
    {
        if (actions.Contains(action))
            actions.Remove(action);
    }

    public static void RemoveAll()
    {
        actions.Clear();
    }

    public static List<IAction> GetActions()
    {
        List<IAction> copy = new();
        foreach (IAction action in actions)
        {
            copy.Add(action);
        }

        return copy;
    }

    public static void RegisterPatternAction(IAction action)
    {
        patternActions.Add(action);
    }

    public static void ShowActionPattern(ActionType actionType, Character character)
    {
        IAction patternAction = patternActions.Find(action => action.ActionType == actionType);
        if (patternAction != null)
            patternAction.ShowActionPattern(character);
    }

    public static void HideAllActionPatterns()
    {
        foreach (IAction action in patternActions)
        {
            action.HideActionPattern();
        }
    }
}
