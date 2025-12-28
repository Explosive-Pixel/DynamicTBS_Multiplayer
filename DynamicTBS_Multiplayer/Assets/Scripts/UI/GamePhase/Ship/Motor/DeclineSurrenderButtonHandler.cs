using UnityEngine;

public class DeclineSurrenderButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject surrenderConfirmationBox;

    public void OnMouseDown()
    {
        if (!GameplayManager.UIPlayerActionAllowed)
            return;

        AudioEvents.PressingButton();
        surrenderConfirmationBox.SetActive(false);
        buttons.SetActive(true);
    }
}
