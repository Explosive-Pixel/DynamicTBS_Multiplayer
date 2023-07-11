using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionUtils : MonoBehaviour
{
    public static List<GameObject> InstantiateActionPositions(List<Vector3> positions, GameObject prefab)
    {
        List<GameObject> actionGameObjects = new List<GameObject>();
        if (positions.Count > 0)
        {
            foreach (Vector3 position in positions)
            {
                actionGameObjects.Add(InstantiateActionPosition(position, prefab));
            }
        }
        return actionGameObjects;
    }

    public static GameObject InstantiateActionPosition(Vector3 position, GameObject prefab)
    {
        prefab.transform.position = new Vector3(position.x, position.y, prefab.transform.position.z);
        return Instantiate(prefab);
    }

    public static List<GameObject> InstantiateActionPositions(List<GameObject> parents, GameObject prefab)
    {
        List<GameObject> actionGameObjects = new List<GameObject>();
        if (parents.Count > 0)
        {
            foreach (GameObject parent in parents)
            {
                GameObject newGO = Instantiate(prefab);
                newGO.transform.parent = parent.transform;
                newGO.transform.position = parent.transform.position;
                actionGameObjects.Add(newGO);
            }
        }
        return actionGameObjects;
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

    public static bool ExecuteAction(Ray ray)
    {
        bool actionExecuted = false;
        foreach (IAction action in ActionRegistry.GetActions())
        {
            GameObject hit = UIUtils.FindGameObjectByRay(action.ActionDestinations, ray);
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

    private static void ExecuteAction(IAction action, GameObject actionDestination)
    {
        Character characterInAction = action.CharacterInAction;
        Vector3 initialPosition = characterInAction.GetCharacterGameObject().transform.position;
        Vector3 actionDestinationPosition = actionDestination.transform.position;

        action.ExecuteAction(actionDestination);

        GameplayEvents.ActionFinished(new ActionMetadata
        {
            ExecutingPlayer = characterInAction.GetSide(),
            ExecutedActionType = action.ActionType,
            CharacterInAction = characterInAction,
            CharacterInitialPosition = initialPosition,
            ActionDestinationPosition = actionDestinationPosition
        });
    }
}
