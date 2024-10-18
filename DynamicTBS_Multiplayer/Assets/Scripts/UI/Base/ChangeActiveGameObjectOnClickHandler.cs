using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeActiveGameObjectOnClickHandler : MonoBehaviour
{
    public BaseActiveHandler activeHandler;
    public GameObject activeOnClickGameObject;

    public void OnMouseDown()
    {
        activeHandler.SetActive(activeOnClickGameObject);
    }
}
