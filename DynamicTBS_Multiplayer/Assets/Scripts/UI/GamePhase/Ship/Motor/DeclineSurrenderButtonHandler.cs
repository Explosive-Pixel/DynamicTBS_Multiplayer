using UnityEngine;

public class DeclineSurrenderButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject surrenderConfirmationBox;

    public void OnMouseDown()
    {
        if (GameManager.IsSpectator() || GameplayManager.gameIsPaused)
            return;

        AudioEvents.PressingButton();
        surrenderConfirmationBox.SetActive(false);
        buttons.SetActive(true);
    }
}
