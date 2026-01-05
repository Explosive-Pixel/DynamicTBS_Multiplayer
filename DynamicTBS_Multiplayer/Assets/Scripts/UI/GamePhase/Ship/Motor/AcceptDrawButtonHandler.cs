using UnityEngine;

public class AcceptDrawButtonHandler : MonoBehaviour
{
    public void OnMouseDown()
    {
        if (!GameplayManager.UIPlayerActionAllowed)
            return;

        AudioEvents.PressingButton();
        GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, UIAction.ACCEPT_DRAW);
    }
}
