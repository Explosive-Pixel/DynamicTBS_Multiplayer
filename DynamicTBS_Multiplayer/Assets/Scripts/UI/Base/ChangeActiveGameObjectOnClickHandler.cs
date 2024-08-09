using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeActiveGameObjectOnClickHandler : MonoBehaviour
{
    [SerializeField] private BaseActiveHandler activeHandler;
    [SerializeField] private GameObject activeOnClickGameObject;

    public void OnMouseDown()
    {
        activeHandler.SetActive(activeOnClickGameObject);
    }
}
