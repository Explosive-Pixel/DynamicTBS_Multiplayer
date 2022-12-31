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
        offerDrawButton.gameObject.SetActive(false);
        answerDrawBox.SetActive(false);
    }

    public void OfferDraw()
    {
        FireUIActionExecutedEvenet(UIActionType.OfferDraw);
    }

    public void AcceptDraw()
    {
        FireUIActionExecutedEvenet(UIActionType.AcceptDraw);
    }

    public void DeclineDraw()
    {
        FireUIActionExecutedEvenet(UIActionType.DeclineDraw);
    }

    private void FireUIActionExecutedEvenet(UIActionType uIActionType)
    {
        Player player = GameManager.gameType == GameType.multiplayer ? PlayerManager.GetPlayer(Client.Instance.side) : PlayerManager.GetCurrentPlayer();
        GameplayEvents.UIActionExecuted(player, uIActionType);
    }

    private void OnDrawButtonClicked(Player player, UIActionType uIActionType)
    {
        if(uIActionType == UIActionType.OfferDraw)
        {
            if (!(GameManager.gameType == GameType.multiplayer && Client.Instance.side == player.GetPlayerType()))
            {
                offerDrawButton.interactable = false;
                answerDrawBox.SetActive(true);
            }
        }
        else if(uIActionType == UIActionType.AcceptDraw)
        {
            GameplayEvents.GameIsOver(null);
        } else if(uIActionType == UIActionType.DeclineDraw)
        {
            offerDrawButton.interactable = true;
            answerDrawBox.SetActive(false);
        }
    }

    private void SetActive()
    {
        offerDrawButton.gameObject.SetActive(true);
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += SetActive;
        GameplayEvents.OnExecuteUIAction += OnDrawButtonClicked;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= SetActive;
        GameplayEvents.OnExecuteUIAction -= OnDrawButtonClicked;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
