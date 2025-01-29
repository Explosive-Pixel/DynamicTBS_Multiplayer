using System.Collections.Generic;
using UnityEngine;

public class UIUtils : MonoBehaviour
{
    public static GameObject FindGameObjectByPosition(List<GameObject> destinations, Vector3 position)
    {
        return destinations.Find(gameObject => HasSamePosition(gameObject, position));
    }

    public static bool HasSamePosition(GameObject gameObject, Vector3 position)
    {
        return gameObject != null && gameObject.transform.position.x == position.x && gameObject.transform.position.y == position.y;
    }

    public static GameObject FindChildGameObject(GameObject parent, string childName)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            if (child.name == childName)
            {
                return child;
            }
        }

        return null;
    }

    public static bool ContainsActive(List<GameObject> gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.activeSelf)
                return true;
        }

        return false;
    }

    public static void UpdateAnimator(Animator animator, int value)
    {
        if (animator != null && animator.parameterCount > 0)
            animator.SetInteger(animator.parameters[0].name, value);
    }
}
