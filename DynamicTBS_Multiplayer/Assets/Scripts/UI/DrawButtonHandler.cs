using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawButtonHandler : MonoBehaviour
{
    [SerializeField] private Button offerDrawButton;

    [SerializeField] private GameObject answerDrawBox;
    [SerializeField] private Button acceptDrawButton;
    [SerializeField] private Button declineDrawButton;

    private void Awake()
    {
        SubscribeEvents();
        SetActive(offerDrawButton.gameObject, false);
        SetActive(answerDrawBox, false);
    }

    public void OfferDraw()
    {
        FireUIActionExecutedEvent(UIActionType.OfferDraw);
    }

    public void AcceptDraw()
    {
        FireUIActionExecutedEvent(UIActionType.AcceptDraw);
    }

    public void DeclineDraw()
    {
        FireUIActionExecutedEvent(UIActionType.DeclineDraw);
    }

    private void FireUIActionExecutedEvent(UIActionType uIActionType)
    {
        Player player = PlayerManager.GetCurrentlyExecutingPlayer();
        GameplayEvents.UIActionExecuted(player, uIActionType);
    }

    private void OnDrawButtonClicked(Player player, UIActionType uIActionType)
    {
        if(uIActionType == UIActionType.OfferDraw)
        {
            offerDrawButton.interactable = false;
            if (!(GameManager.gameType == GameType.multiplayer && Client.Instance.side == player.GetPlayerType()))
            {
                SetActive(answerDrawBox, true);
            }
        }
        else if(uIActionType == UIActionType.AcceptDraw)
        {
            GameplayEvents.GameIsOver(null, GameOverCondition.DRAW_ACCEPTED);
        } else if(uIActionType == UIActionType.DeclineDraw)
        {
            offerDrawButton.interactable = true;
            SetActive(answerDrawBox, false);
        }
    }

    private void SetOfferDrawButtonActive()
    {
        SetActive(offerDrawButton.gameObject, true);
    }

    private void SetActive(GameObject gameObject, bool active)
    {
        if (GameManager.gameType == GameType.multiplayer && Client.Instance.role == ClientType.spectator)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(active);
        }
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += SetOfferDrawButtonActive;
        GameplayEvents.OnExecuteUIAction += OnDrawButtonClicked;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= SetOfferDrawButtonActive;
        GameplayEvents.OnExecuteUIAction -= OnDrawButtonClicked;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
