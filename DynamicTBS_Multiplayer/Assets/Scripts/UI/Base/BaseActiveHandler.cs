using UnityEngine;
using UnityEngine.UI;

public class BaseActiveHandler : MonoBehaviour
{
    private GameObject currentActiveGameObject;
    private Button lastClickedButton;

    public void SetActive(GameObject gameObject)
    {
        SetInactive();

        gameObject.SetActive(true);
        currentActiveGameObject = gameObject;
    }

    public void SetActive(Button clickedButton, GameObject gameObject)
    {
        SetActive(gameObject);

        clickedButton.interactable = false;
        lastClickedButton = clickedButton;
    }

    public void SetInactive()
    {
        if (currentActiveGameObject != null)
        {
            currentActiveGameObject.SetActive(false);
            currentActiveGameObject = null;
        }
        if (lastClickedButton != null)
        {
            lastClickedButton.interactable = true;
        }
    }
}
