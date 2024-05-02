using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceptSurrenderButtonHandler : MonoBehaviour
{
    public void OnMouseDown()
    {
        if (GameManager.IsSpectator())
            return;

        PlayerType player = PlayerManager.ExecutingPlayer;
        GameplayEvents.UIActionExecuted(player, UIAction.SURRENDER);
    }
}
