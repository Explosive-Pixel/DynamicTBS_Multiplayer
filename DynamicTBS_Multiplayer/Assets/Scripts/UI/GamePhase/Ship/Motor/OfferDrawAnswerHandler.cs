using UnityEngine;

public class OfferDrawAnswerHandler : MonoBehaviour
{
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject answerDrawBox;
    [SerializeField] private TMPro.TextMeshPro answerDrawBoxTimer;
    [SerializeField] private GameObject drawInfoBox;
    [SerializeField] private TMPro.TextMeshPro drawInfoBoxTimer;
    [SerializeField] private float offerDrawTime;

    private float remainingTime = 0;

    private void Awake()
    {
        SubscribeEvents();
        SetInactive();
    }

    private void Update()
    {
        if (remainingTime <= 0)
            return;

        remainingTime -= Time.deltaTime;
        UpdateTimer(remainingTime);

        if (remainingTime <= 0)
        {
            SetInactive();
            buttons.SetActive(true);
        }
    }

    /* public void OnClick()
     {
         GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, UIAction.OFFER_DRAW);
     }*/

    private void OnDrawButtonClicked(PlayerType player, UIAction uIAction)
    {
        if (uIAction == UIAction.OFFER_DRAW)
        {
            if (!PlayerSetup.IsSide(player))
            {
                SetActive(answerDrawBox, true);
            }
            else
            {
                SetActive(drawInfoBox, true);
            }

            buttons.SetActive(false);
            StartOfferDrawTimer();
        }
        else if (uIAction == UIAction.ACCEPT_DRAW)
        {
            GameplayEvents.GameIsOver(null, GameOverCondition.DRAW_ACCEPTED);
        }
        else if (uIAction == UIAction.DECLINE_DRAW)
        {
            SetInactive();
            buttons.SetActive(true);
        }
    }

    private void StartOfferDrawTimer()
    {
        remainingTime = offerDrawTime;
        UpdateTimer(remainingTime);
    }

    private void UpdateTimer(float time)
    {
        string text = Mathf.FloorToInt(time) + "...";
        answerDrawBoxTimer.text = text;
        drawInfoBoxTimer.text = text;
    }

    private void SetActive(GameObject gameObject, bool active)
    {
        gameObject.SetActive(GameManager.IsPlayer() && active);
    }

    private void SetInactive()
    {
        remainingTime = 0;
        SetActive(answerDrawBox, false);
        SetActive(drawInfoBox, false);
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnExecuteUIAction += OnDrawButtonClicked;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnExecuteUIAction -= OnDrawButtonClicked;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
