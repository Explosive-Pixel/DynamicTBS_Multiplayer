using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUtils : MonoBehaviour
{
    public static GameObject FindGameObjectByPosition(List<GameObject> destinations, Vector3 position)
    {
        return destinations.Find(gameObject => HasSamePosition(gameObject, position));
        //return FindGameObjectByRay(destinations, DefaultRay(position));
    }

    public static bool HasSamePosition(GameObject gameObject, Vector3 position)
    {
        return gameObject.transform.position.x == position.x && gameObject.transform.position.y == position.y;
    }

    public static Ray DefaultRay(Vector3 position)
    {
        position.z = 0f;
        return new Ray(position, Vector3.forward);
    }

    public static GameObject FindGameObjectByRay(List<GameObject> destinations, Ray ray)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray);
        GameObject hitWithMaxZValue = null;

        if (hits != null && hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                GameObject gameObject = hit.transform.gameObject;
                if (destinations.Contains(gameObject))
                {
                    if (hitWithMaxZValue == null || gameObject.transform.position.z > hitWithMaxZValue.transform.position.z)
                        hitWithMaxZValue = gameObject;
                }
            }
        }

        return hitWithMaxZValue;
    }

    public static bool IsHit()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null &&
                     UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
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

    public static void UpdateAnimator(Animator animator, int value)
    {
        if (animator != null && animator.parameterCount > 0)
            animator.SetInteger(animator.parameters[0].name, value);
    }
}
