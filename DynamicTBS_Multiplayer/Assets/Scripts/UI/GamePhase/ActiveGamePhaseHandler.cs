using System.Collections.Generic;
using UnityEngine;

public class ActiveGamePhaseHandler : MonoBehaviour
{
    [SerializeField] private List<GamePhase> activeGamePhases;
    [SerializeField] private bool setInactiveOnGamePhaseEnd = false;

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += SetActive;

        if (setInactiveOnGamePhaseEnd)
            GameEvents.OnGamePhaseEnd += SetInactive;
    }

    private void SetActive(GamePhase gamePhase)
    {

        gameObject.SetActive(activeGamePhases.Contains(gamePhase));
    }

    private void SetInactive(GamePhase gamePhase)
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= SetActive;
        GameEvents.OnGamePhaseEnd -= SetInactive;
    }
}
