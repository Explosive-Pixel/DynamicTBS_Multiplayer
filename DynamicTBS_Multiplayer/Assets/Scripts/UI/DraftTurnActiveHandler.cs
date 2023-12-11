using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftTurnActiveHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> turnHighlights;

    private int counter = 0;

    private void Awake()
    {
        turnHighlights.ForEach(go => go.SetActive(false));
        SetActive(0);

        GameplayEvents.OnCurrentPlayerChanged += UpdateTurnHighlights;
        GameEvents.OnGamePhaseEnd += UnsubscribeEvents;
    }

    private void UpdateTurnHighlights(PlayerType nextPlayer)
    {
        turnHighlights[counter].SetActive(false);
        SetActive(++counter);
    }

    private void SetActive(int turnHighlightIndex)
    {
        if (turnHighlightIndex < turnHighlights.Count)
            turnHighlights[turnHighlightIndex].SetActive(true);
    }

    private void UnsubscribeEvents(GamePhase gamePhase)
    {
        OnDestroy();
    }

    private void OnDestroy()
    {
        GameplayEvents.OnCurrentPlayerChanged -= UpdateTurnHighlights;
        GameEvents.OnGamePhaseEnd -= UnsubscribeEvents;
    }
}
