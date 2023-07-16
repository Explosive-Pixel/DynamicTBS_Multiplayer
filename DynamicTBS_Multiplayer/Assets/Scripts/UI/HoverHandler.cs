using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
