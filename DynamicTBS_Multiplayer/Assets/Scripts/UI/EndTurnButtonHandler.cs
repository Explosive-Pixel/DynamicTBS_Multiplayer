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
        ChangeButtonVisibilityOnMultiplayer();
        turnEndedButton.interactable = GameplayManager.GetRemainingActions() == 1;
    }

    private void ChangeButtonVisibility(bool active)
    {
        turnEndedButton.gameObject.SetActive(active);
    }

    private void ChangeButtonVisibilityOnMultiplayer()
    {
        if (GameManager.gameType == GameType.multiplayer)
        {
            ChangeButtonVisibility(PlayerManager.GetCurrentPlayer().GetPlayerType() == Client.Instance.side);
        }
    }

    private void SetActive()
    {
        if (GameManager.gameType == GameType.multiplayer && PlayerManager.GameplayPhaseStartPlayer != Client.Instance.side)
        {
            ChangeButtonVisibility(false);
        }
        else
        {
            ChangeButtonVisibility(true);
        }
        turnEndedButton.interactable = false;
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
