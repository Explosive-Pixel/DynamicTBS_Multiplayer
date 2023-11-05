using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeclineDrawButtonHandler : MonoBehaviour, IClickableObject
{
    public void OnClick()
    {
        GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, UIAction.DECLINE_DRAW);
    }
}