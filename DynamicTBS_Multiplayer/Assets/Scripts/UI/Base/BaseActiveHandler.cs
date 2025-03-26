using UnityEngine;
using UnityEngine.UI;

public class BaseActiveHandler : MonoBehaviour
{
    private GameObject currentActiveGameObject;
    private Button lastClickedButton;

    public void SetActive(GameObject gameObject)
    {
        if (currentActiveGameObject)
            currentActiveGameObject.SetActive(false);

        gameObject.SetActive(true);
        currentActiveGameObject = gameObject;
    }

    public void SetActive(Button clickedButton, GameObject gameObject)
    {
        if (lastClickedButton != null)
            lastClickedButton.interactable = true;
        clickedButton.interactable = false;
        lastClickedButton = clickedButton;

        SetActive(gameObject);
    }

    public void SetInactive()
    {
        if (currentActiveGameObject != null)
        {
            currentActiveGameObject.SetActive(false);
            currentActiveGameObject = null;
        }
    }
}
