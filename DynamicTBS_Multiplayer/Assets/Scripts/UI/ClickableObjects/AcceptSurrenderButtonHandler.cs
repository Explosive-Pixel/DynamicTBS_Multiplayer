using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceptSurrenderButtonHandler : MonoBehaviour, IClickableObject
{
    [SerializeField] private ClickPermission clickPermission;

    public ClickPermission ClickPermission { get { return clickPermission; } }

    public void OnClick()
    {
        PlayerType player = PlayerManager.ExecutingPlayer;
        GameplayEvents.UIActionExecuted(player, UIAction.SURRENDER);
    }
}
