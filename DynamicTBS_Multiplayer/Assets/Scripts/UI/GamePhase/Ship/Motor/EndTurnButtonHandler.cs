using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject turnEndedButton;
    [SerializeField] private GameObject turnEndedButton_grayed;

    private void Awake()
    {
        SubscribeEvents();
        SetActive(false, true);
    }

    public void OnMouseDown()
    {
        if (!PlayerManager.ClientIsCurrentPlayer() || GameplayManager.gameIsPaused)
            return;

        if (turnEndedButton.activeSelf)
        {
            AudioEvents.PressingButton();
            SkipAction.Execute();
        }
    }

    private void SetActive(bool active, bool interactable)
    {
        turnEndedButton.SetActive(active && interactable);
        turnEndedButton_grayed.SetActive(active && !interactable);
    }

    private void ChangeButtonInteractability()
    {
        SetActive(!PlayerSetup.IsNotSide(PlayerManager.CurrentPlayer), GameplayManager.GetRemainingActions() == 1);
    }

    private void SetActive(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.GAMEPLAY)
            return;

        SetActive(GameManager.IsPlayer() && !PlayerSetup.IsNotSide(PlayerManager.StartPlayer[GamePhase.GAMEPLAY]), false);
        GameplayEvents.OnChangeRemainingActions += ChangeButtonInteractability;
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += SetActive;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= SetActive;
        GameplayEvents.OnChangeRemainingActions -= ChangeButtonInteractability;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
