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
        if (!GameManager.IsPlayer())
        {
            turnEndedButton.gameObject.SetActive(false);
        }
        else
        {
            turnEndedButton.gameObject.SetActive(active);
        }
    }

    private void ChangeButtonVisibilityOnMultiplayer()
    {
        if (GameManager.gameType == GameType.ONLINE)
        {
            ChangeButtonVisibility(PlayerManager.CurrentPlayer == OnlineClient.Instance.Side);
        }
    }

    private void SetActive(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.GAMEPLAY)
            return;

        if (GameManager.gameType == GameType.ONLINE && PlayerManager.StartPlayer[GamePhase.GAMEPLAY] != OnlineClient.Instance.Side)
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
