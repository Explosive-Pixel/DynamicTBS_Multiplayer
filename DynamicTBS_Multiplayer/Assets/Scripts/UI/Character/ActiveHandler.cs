using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveHandler : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectActive;
    [SerializeField] private GameObject gameObjectInactive;

    public void SetActive(bool active)
    {
        gameObjectActive.SetActive(active);
        gameObjectInactive.SetActive(!active);
    }
}
