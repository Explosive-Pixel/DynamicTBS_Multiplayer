using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHandler : MonoBehaviour
{
    [SerializeField] private GameObject hoverObject;

    private void OnMouseEnter()
    {
        hoverObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        hoverObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        hoverObject.SetActive(false);
    }

    private void OnDisable()
    {
        hoverObject.SetActive(false);
    }
}
