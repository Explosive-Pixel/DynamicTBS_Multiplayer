using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfferDrawButtonHandler : MonoBehaviour, IClickableObject
{
    [SerializeField] private ClickPermission clickPermission;

    public ClickPermission ClickPermission { get { return clickPermission; } }

    private void Awake()
    {
        SubscribeEvents();
    }

    public void OnClick()
    {
        GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, UIAction.OFFER_DRAW);
    }

    private void SetOfferDrawButtonActive(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.GAMEPLAY)
            SetActive(gameObject, true);
    }

    private void SetActive(GameObject gameObject, bool active)
    {
        gameObject.SetActive(GameManager.IsPlayer() && active);
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += SetOfferDrawButtonActive;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= SetOfferDrawButtonActive;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
