using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoverHandler : MonoBehaviour
{
    [SerializeField] private GameObject hoverObject;
    [SerializeField] private GameObject activationHover;

    [SerializeField] private UnityEvent myUnityEvent;

    private void Start()
    {
        SetActive(false);
    }

    private void OnMouseEnter()
    {
        SetActive(true);
    }

    private void OnMouseExit()
    {
        SetActive(false);
    }

    private void OnMouseDown()
    {
        SetActive(false);
    }

    private void OnDisable()
    {
        SetActive(false);
    }

    private void SetActive(bool active)
    {
        hoverObject.SetActive(active);
        activationHover.SetActive(active);

        if (!active)
            return;

        myUnityEvent?.Invoke();
    }
}
