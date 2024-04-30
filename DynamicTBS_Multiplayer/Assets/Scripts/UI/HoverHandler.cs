using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoverHandler : MonoBehaviour
{
    [SerializeField] private GameObject hoverObject;
    [SerializeField] private GameObject activationHover;

    [SerializeField] private UnityEvent onHover;
    [SerializeField] private UnityEvent onDrop;

    private bool clicked = false;
    private bool disabled = false;

    private void Start()
    {
        SetActive(false);
    }

    private void OnEnable()
    {
        disabled = false;
    }

    private void OnMouseEnter()
    {
        clicked = false;
        SetActive(true);
    }

    private void OnMouseExit()
    {
        SetActive(false);
    }

    private void OnMouseDown()
    {
        SetActive(false);
        clicked = true;
    }

    private void OnDisable()
    {
        disabled = true;
        SetActive(false);
    }

    private void SetActive(bool active)
    {
        if (hoverObject != null)
            hoverObject.SetActive(active);

        if (activationHover != null)
            activationHover.SetActive(active);

        if (clicked || disabled)
            return;

        if (active)
            onHover?.Invoke();
        else
            onDrop?.Invoke();
    }
}
