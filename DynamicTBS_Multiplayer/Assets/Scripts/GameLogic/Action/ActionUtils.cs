using System.Collections.Generic;
using UnityEngine;

public class ActionUtils : MonoBehaviour
{
    public static List<GameObject> InstantiateActionPositions(IAction action, List<Vector3> positions, GameObject prefab)
    {
        List<GameObject> actionGameObjects = new();
        if (positions.Count > 0)
        {
            foreach (Vector3 position in positions)
            {
                actionGameObjects.Add(InstantiateActionPosition(action, position, prefab));
            }
        }
        return actionGameObjects;
    }

    public static GameObject InstantiateActionPosition(IAction action, Vector3 position, GameObject prefab)
    {
        GameObject actionPosition = Instantiate(prefab);
        actionPosition.transform.position = new Vector3(position.x, position.y, prefab.transform.position.z);
        ActionTileHandler.Create(action, actionPosition);
        return actionPosition;
    }

    public static void Clear(List<GameObject> actionGameObjects)
    {
        foreach (GameObject gameobject in actionGameObjects)
        {
            Destroy(gameobject);
        }
        actionGameObjects.Clear();
    }

    public static void InstantiateAllActionPositions(Character character)
    {
        AbortAllActions();
        foreach (IAction action in ActionRegistry.GetActions())
        {
            if (GameplayManager.ActionAvailable(character, action.ActionType) && !character.isDisabled())
                action.CreateActionDestinations(character);
        }
    }

    public static int CountAllActionDestinations(Character character)
    {
        int actionDestinationCount = 0;

        foreach (IAction action in ActionRegistry.GetActions())
        {
            if (GameplayManager.ActionAvailable(character, action.ActionType))
                actionDestinationCount += action.CountActionDestinations(character);
        }

        return actionDestinationCount;
    }

    public static bool ExecuteAction(Vector3 position)
    {
        bool actionExecuted = false;
        foreach (IAction action in ActionRegistry.GetActions())
        {
            GameObject hit = UIUtils.FindGameObjectByPosition(action.ActionDestinations, position);
            if (hit != null)
            {
                ExecuteAction(action, hit);
                actionExecuted = true;
            }
            else
                action.AbortAction();
        }

        return actionExecuted;
    }

    public static void ExecuteAction(GameObject actionDestination)
    {
        foreach (IAction action in ActionRegistry.GetActions())
        {
            if (action.ActionDestinations.Contains(actionDestination))
            {
                ExecuteAction(action, actionDestination);
            }
        }
    }

    public static void ResetActionDestinations()
    {
        AbortAllActions();
        HideAllActionPatterns();
    }

    public static void AbortAllActions()
    {
        foreach (IAction action in ActionRegistry.GetActions())
        {
            action.AbortAction();
        }
    }

    public static void HideAllActionPatterns()
    {
        ActionRegistry.HideAllActionPatterns();
    }

    public static void ExecuteAction(IAction action, GameObject actionDestination)
    {
        Character characterInAction = action.CharacterInAction;
        Vector3 initialPosition = characterInAction.gameObject.transform.position;
        Vector3 actionDestinationPosition = actionDestination.transform.position;

        action.ExecuteAction(actionDestination);
        ResetActionDestinations();

        GameplayEvents.ActionFinished(new ActionMetadata
        {
            ExecutingPlayer = characterInAction.Side,
            ExecutedActionType = action.ActionType,
            CharacterInAction = characterInAction,
            CharacterInitialPosition = initialPosition,
            ActionDestinationPosition = actionDestinationPosition
        });
    }
}
