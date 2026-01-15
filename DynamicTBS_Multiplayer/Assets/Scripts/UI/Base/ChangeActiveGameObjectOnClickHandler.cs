using UnityEngine;
using UnityEngine.UI;

public class ChangeActiveGameObjectOnClickHandler : MonoBehaviour
{
    public BaseActiveHandler activeHandler;
    public GameObject activeOnClickGameObject;

    public void OnMouseDown()
    {
        AudioEvents.PressingButton();

        if (gameObject.GetComponent<Button>() != null)
        {
            activeHandler.SetActive(gameObject.GetComponent<Button>(), activeOnClickGameObject);
        }
        else
        {
            activeHandler.SetActive(activeOnClickGameObject);
        }
    }
}
