using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHandler : MonoBehaviour
{
    [SerializeField] private GameObject hoverObject;
    [SerializeField] private GameObject activationHover;

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
    }
}
