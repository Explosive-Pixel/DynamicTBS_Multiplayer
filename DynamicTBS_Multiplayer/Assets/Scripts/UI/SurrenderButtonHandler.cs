using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurrenderButtonHandler : MonoBehaviour, IClickableObject
{
    private void Awake()
    {
        SubscribeEvents();
        ChangeButtonVisibility(false);
    }

    public void OnClick()
    {
        PlayerType player = PlayerManager.ExecutingPlayer;
        GameplayEvents.UIActionExecuted(player, UIAction.SURRENDER);
    }

    private void OnSurrenderClicked(PlayerType player, UIAction uIAction)
    {
        if (uIAction == UIAction.SURRENDER)
            GameplayEvents.GameIsOver(PlayerManager.GetOtherSide(player), GameOverCondition.PLAYER_SURRENDERED);
    }

    private void ChangeButtonVisibility(bool active)
    {
        gameObject.SetActive(GameManager.IsPlayer() && active);
    }

    private void SetActive(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.GAMEPLAY)
            ChangeButtonVisibility(true);
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += SetActive;
        GameplayEvents.OnExecuteUIAction += OnSurrenderClicked;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= SetActive;
        GameplayEvents.OnExecuteUIAction -= OnSurrenderClicked;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
