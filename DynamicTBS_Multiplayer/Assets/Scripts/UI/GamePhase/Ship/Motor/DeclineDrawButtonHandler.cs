using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeclineDrawButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject buttons;

    public void OnMouseDown()
    {
        if (GameManager.IsSpectator() || GameplayManager.gameIsPaused)
            return;

        AudioEvents.PressingButton();
        GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, UIAction.DECLINE_DRAW);
        buttons.SetActive(true);
    }
}