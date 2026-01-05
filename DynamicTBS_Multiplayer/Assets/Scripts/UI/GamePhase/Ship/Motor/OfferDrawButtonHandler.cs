using UnityEngine;

public class OfferDrawButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject buttons;

    private void Awake()
    {
        SubscribeEvents();
    }

    public void OnMouseDown()
    {
        if (!GameplayManager.UIPlayerActionAllowed)
            return;

        AudioEvents.PressingButton();
        buttons.SetActive(false);
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
