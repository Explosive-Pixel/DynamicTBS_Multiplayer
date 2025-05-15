using System.Collections.Generic;

public class PlayerActionRegistry
{
    private static readonly List<IPlayerAction> actions = new();

    public static void Register(IPlayerAction action)
    {
        actions.Add(action);
    }

    public static void Remove(IPlayerAction action)
    {
        if (actions.Contains(action))
            actions.Remove(action);
    }

    public static void RemoveAll()
    {
        actions.Clear();
    }

    public static List<IPlayerAction> GetActions()
    {
        List<IPlayerAction> copy = new();
        foreach (IPlayerAction action in actions)
        {
            copy.Add(action);
        }

        return copy;
    }

    public static IPlayerAction GetAction(PlayerActionType actionType)
    {
        return actions.Find(a => a.PlayerActionType == actionType);
    }
}
