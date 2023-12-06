using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfferDrawButtonHandler : MonoBehaviour, IClickableObject
{
    [SerializeField] private GameObject answerDrawBox;
    [SerializeField] private ClickPermission clickPermission;

    public ClickPermission ClickPermission { get { return clickPermission; } }

    private void Awake()
    {
        SubscribeEvents();
        SetActive(gameObject, false);
        SetActive(answerDrawBox, false);
    }

    public void OnClick()
    {
        GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, UIAction.OFFER_DRAW);
    }

    private void OnDrawButtonClicked(PlayerType player, UIAction uIAction)
    {
        if (uIAction == UIAction.OFFER_DRAW)
        {
            if (!(GameManager.GameType == GameType.ONLINE && OnlineClient.Instance.Side == player))
            {
                SetActive(answerDrawBox, true);
            }
        }
        else if (uIAction == UIAction.ACCEPT_DRAW)
        {
            GameplayEvents.GameIsOver(null, GameOverCondition.DRAW_ACCEPTED);
        }
        else if (uIAction == UIAction.DECLINE_DRAW)
        {
            SetActive(answerDrawBox, false);
        }
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
        GameplayEvents.OnExecuteUIAction += OnDrawButtonClicked;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= SetOfferDrawButtonActive;
        GameplayEvents.OnExecuteUIAction -= OnDrawButtonClicked;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
