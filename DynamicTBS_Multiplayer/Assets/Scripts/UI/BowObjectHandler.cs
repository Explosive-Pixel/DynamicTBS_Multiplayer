using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowObjectHandler : MonoBehaviour
{
    [SerializeField] private GameObject bowDisplays;

    private void Awake()
    {
        bowDisplays.SetActive(false);

        GameEvents.OnGamePhaseStart += SetActive;
    }

    private void SetActive(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.GAMEPLAY)
            return;

        bowDisplays.SetActive(true);
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= SetActive;
    }
}
