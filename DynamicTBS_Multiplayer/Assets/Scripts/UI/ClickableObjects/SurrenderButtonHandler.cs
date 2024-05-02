using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurrenderButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject surrenderConfirmationBox;

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += SetActive;
        GameplayEvents.OnExecuteUIAction += OnSurrenderClicked;

        ChangeButtonVisibility(false);
        surrenderConfirmationBox.SetActive(false);
    }

    public void OnMouseDown()
    {
        if (GameManager.IsSpectator())
            return;

        surrenderConfirmationBox.SetActive(true);
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

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= SetActive;
        GameplayEvents.OnExecuteUIAction -= OnSurrenderClicked;
    }
}
