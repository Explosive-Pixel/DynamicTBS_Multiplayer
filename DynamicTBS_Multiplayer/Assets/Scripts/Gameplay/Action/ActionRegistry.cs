using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRegistry
{
    private static readonly List<IAction> actions = new List<IAction>();

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
        List<IAction> copy = new List<IAction>();
        foreach(IAction action in actions)
        {
            copy.Add(action);
        }

        return copy;
    }
}
