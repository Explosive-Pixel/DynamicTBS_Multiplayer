using UnityEngine;

public class AcceptSurrenderButtonHandler : MonoBehaviour
{
    public void OnMouseDown()
    {
        if (GameManager.IsSpectator() || GameplayManager.gameIsPaused)
            return;

        AudioEvents.PressingButton();

        PlayerType player = PlayerManager.ExecutingPlayer;
        GameplayEvents.UIActionExecuted(player, UIAction.SURRENDER);
    }
}
