using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceptDrawButtonHandler : MonoBehaviour
{
    public void OnMouseDown()
    {
        if (GameManager.IsSpectator())
            return;

        GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, UIAction.ACCEPT_DRAW);
    }
}
