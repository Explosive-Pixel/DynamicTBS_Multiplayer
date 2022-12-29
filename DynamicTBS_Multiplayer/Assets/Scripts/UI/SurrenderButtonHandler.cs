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
        GameplayEvents.GameIsOver(PlayerManager.GetOtherPlayer(PlayerManager.GetCurrentPlayer()).GetPlayerType());
    }

    private void ChangeButtonVisibility(bool active)
    {
        surrenderButton.gameObject.SetActive(active);
    }

    private void SetActive()
    {
        ChangeButtonVisibility(true);
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += SetActive;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= SetActive;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
