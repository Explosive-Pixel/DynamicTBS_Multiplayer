using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurrenderButtonHandler : MonoBehaviour
{
    [SerializeField] private Button surrenderButton;

    private void Awake()
    {
        SubscribeEvents();
        ChangeButtonVisibility(false);
    }

    public void Surrender()
    {
        Player player = PlayerManager.GetCurrentlyExecutingPlayer();
        GameplayEvents.UIActionExecuted(player, UIAction.SURRENDER);
    }

    private void OnSurrenderClicked(Player player, UIAction uIAction)
    {
        if(uIAction == UIAction.SURRENDER)
            GameplayEvents.GameIsOver(PlayerManager.GetOtherPlayer(player).GetPlayerType(), GameOverCondition.PLAYER_SURRENDERED);
    }

    private void ChangeButtonVisibility(bool active)
    {
        if (!GameManager.IsPlayer())
        {
            surrenderButton.gameObject.SetActive(false);
        }
        else
        {
            surrenderButton.gameObject.SetActive(active);
        }
    }

    private void SetActive()
    {
        ChangeButtonVisibility(true);
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += SetActive;
        GameplayEvents.OnExecuteUIAction += OnSurrenderClicked;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= SetActive;
        GameplayEvents.OnExecuteUIAction -= OnSurrenderClicked;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
