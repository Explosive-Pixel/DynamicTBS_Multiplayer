using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeclineDrawButtonHandler : MonoBehaviour
{
    public void OnMouseDown()
    {
        if (GameManager.IsSpectator())
            return;

        GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, UIAction.DECLINE_DRAW);
    }
}