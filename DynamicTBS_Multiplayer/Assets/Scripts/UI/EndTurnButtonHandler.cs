using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButtonHandler : MonoBehaviour
{
    [SerializeField] private Button turnEndedButton;

    private void Awake()
    {
        SubscribeEvents();
        ChangeButtonVisibility(false);
    }

    public void FinishTurn()
    {
        SkipAction.Execute();
    }

    private void ChangeButtonInteractability()
    {
        turnEndedButton.interactable = GameplayManager.GetRemainingActions() == 1;
    }

    private void ChangeButtonVisibility(bool active)
    {
        turnEndedButton.gameObject.SetActive(active);
    }

    private void SetActive()
    {
        ChangeButtonVisibility(true);
        ChangeButtonInteractability();
        GameplayEvents.OnChangeRemainingActions += ChangeButtonInteractability;
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += SetActive;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= SetActive;
        GameplayEvents.OnChangeRemainingActions -= ChangeButtonInteractability;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
