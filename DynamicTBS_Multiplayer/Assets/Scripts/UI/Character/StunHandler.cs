using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunHandler : MonoBehaviour
{
    [SerializeField] private GameObject stunMarker;
    [SerializeField] private GameObject characterSprite;
    [SerializeField] private Color stunColor;

    private Color defaultColor;

    private void Awake()
    {
        defaultColor = characterSprite.GetComponentInChildren<SpriteRenderer>().color;
    }

    public void VisualizeStun(bool active)
    {
        stunMarker.SetActive(active);
        characterSprite.GetComponentInChildren<SpriteRenderer>().color = active ? stunColor : defaultColor;
    }
}
