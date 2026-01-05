using UnityEngine;

public class DeclineDrawButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject buttons;

    public void OnMouseDown()
    {
        if (!GameplayManager.UIPlayerActionAllowed)
            return;

        AudioEvents.PressingButton();
        GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, UIAction.DECLINE_DRAW);
        buttons.SetActive(true);
    }
}