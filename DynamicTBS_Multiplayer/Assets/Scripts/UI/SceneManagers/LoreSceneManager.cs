using System.Collections.Generic;
using UnityEngine;

public class LoreSceneManager : MonoBehaviour
{
    private List<GameObject> canvasList = new List<GameObject>();

    public GameObject introductionCanvas;

    private void Awake()
    {
        SetCanvasList();
        GoToCanvas(introductionCanvas);
    }

    private void SetCanvasList()
    {
        canvasList.Add(introductionCanvas);
    }

    private void GoToCanvas(GameObject canvas)
    {
        AudioEvents.PressingButton();

        foreach (GameObject gameObject in canvasList)
        {
            if (canvas == gameObject)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}