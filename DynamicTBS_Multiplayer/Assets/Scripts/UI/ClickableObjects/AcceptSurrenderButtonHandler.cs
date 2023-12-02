using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceptSurrenderButtonHandler : MonoBehaviour, IClickableObject
{
    [SerializeField] private ClickPermission clickPermission;

    public ClickPermission ClickPermission { get { return clickPermission; } }

    private void Awake()
    {
        GameplayEvents.OnExecuteUIAction += OnSurrenderClicked;
    }

    public void OnClick()
    {
        PlayerType player = PlayerManager.ExecutingPlayer;
        GameplayEvents.UIActionExecuted(player, UIAction.SURRENDER);
    }

    private void OnSurrenderClicked(PlayerType player, UIAction uIAction)
    {
        if (uIAction == UIAction.SURRENDER)
            GameplayEvents.GameIsOver(PlayerManager.GetOtherSide(player), GameOverCondition.PLAYER_SURRENDERED);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnExecuteUIAction -= OnSurrenderClicked;
    }
}
