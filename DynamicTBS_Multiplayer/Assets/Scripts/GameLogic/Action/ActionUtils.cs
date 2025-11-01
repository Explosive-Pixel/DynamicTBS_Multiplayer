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
        Tile tile = Board.GetTileByPosition(position);
        if (tile != null)
            actionPosition.transform.SetParent(tile.gameObject.transform);
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
}
