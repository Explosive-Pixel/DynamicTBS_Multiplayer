using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseActiveHandler : MonoBehaviour
{
    private GameObject currentActiveGameObject;

    public void SetActive(GameObject gameObject)
    {
        if (currentActiveGameObject)
            currentActiveGameObject.SetActive(false);

        gameObject.SetActive(true);
        currentActiveGameObject = gameObject;
    }
}
