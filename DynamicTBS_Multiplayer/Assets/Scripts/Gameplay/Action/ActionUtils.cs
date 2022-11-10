using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        prefab.transform.position = new Vector3(position.x, position.y, 0.995f);
        return Instantiate(prefab);
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
        foreach (IAction action in ActionRegistry.GetActions())
        {
            if (GameplayManager.ActionAvailable(character, action.ActionType))
                action.CreateActionDestinations(character);
        }
    }

    public static bool ExecuteAction(Ray ray)
    {
        bool actionExecuted = false;
        foreach (IAction action in ActionRegistry.GetActions())
        {
            GameObject hit = UIUtils.FindGameObjectByRay(action.ActionDestinations, ray);
            if (hit != null)
            {
                Character characterInAction = action.CharacterInAction;
                Vector3 initialPosition = characterInAction.GetCharacterGameObject().transform.position;
                Vector3 actionDestinationPosition = hit.transform.position;

                action.ExecuteAction(hit);

                GameplayEvents.ActionFinished(characterInAction, action.ActionType, initialPosition, actionDestinationPosition);
                actionExecuted = true;
            }
            else
                action.AbortAction();
        }

        return actionExecuted;
    }
}
