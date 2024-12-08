using UnityEngine;

public class AcceptDrawButtonHandler : MonoBehaviour
{
    public void OnMouseDown()
    {
        if (GameManager.IsSpectator() || GameplayManager.gameIsPaused)
            return;

        AudioEvents.PressingButton();
        GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, UIAction.ACCEPT_DRAW);
    }
}
