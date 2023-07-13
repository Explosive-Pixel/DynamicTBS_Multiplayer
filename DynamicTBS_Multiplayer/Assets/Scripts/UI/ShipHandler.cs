using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHandler : MonoBehaviour
{
    private void Awake()
    {
        GameEvents.OnGamePhaseStart += ShowShip;
        gameObject.SetActive(false);
    }

    private void ShowShip(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.PLACEMENT)
        {
            gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= ShowShip;
    }
}
