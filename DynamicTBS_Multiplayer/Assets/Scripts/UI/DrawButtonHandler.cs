using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawButtonHandler : MonoBehaviour, IClickableObject
{
    [SerializeField] private GameObject answerDrawBox;
    [SerializeField] private Button acceptDrawButton;
    [SerializeField] private Button declineDrawButton;

    private void Awake()
    {
        SubscribeEvents();
        SetActive(gameObject, false);
        SetActive(answerDrawBox, false);
    }

    public void OnClick()
    {
        FireUIActionExecutedEvent(UIAction.OFFER_DRAW);
    }

    public void AcceptDraw()
    {
        FireUIActionExecutedEvent(UIAction.ACCEPT_DRAW);
    }

    public void DeclineDraw()
    {
        FireUIActionExecutedEvent(UIAction.DECLINE_DRAW);
    }

    private void FireUIActionExecutedEvent(UIAction uIAction)
    {
        PlayerType player = PlayerManager.ExecutingPlayer;
        GameplayEvents.UIActionExecuted(player, uIAction);
    }

    private void OnDrawButtonClicked(PlayerType player, UIAction uIAction)
    {
        if (uIAction == UIAction.OFFER_DRAW)
        {
            //offerDrawButton.interactable = false;
            if (!(GameManager.gameType == GameType.ONLINE && OnlineClient.Instance.Side == player))
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
            //offerDrawButton.interactable = true;
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
