using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveGamePhaseHandler : MonoBehaviour
{
    [SerializeField] private List<GamePhase> activeGamePhases;

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += SetActive;
    }

    private void SetActive(GamePhase gamePhase)
    {
        gameObject.SetActive(activeGamePhases.Contains(gamePhase));
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= SetActive;
    }
}
