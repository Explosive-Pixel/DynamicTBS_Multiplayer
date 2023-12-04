using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeclineDrawButtonHandler : MonoBehaviour, IClickableObject
{
    [SerializeField] private ClickPermission clickPermission;

    public ClickPermission ClickPermission { get { return clickPermission; } }

    public void OnClick()
    {
        GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, UIAction.DECLINE_DRAW);
    }
}