using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceptDrawButtonHandler : MonoBehaviour, IClickableObject
{
    public void OnClick()
    {
        GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, UIAction.ACCEPT_DRAW);
    }
}
