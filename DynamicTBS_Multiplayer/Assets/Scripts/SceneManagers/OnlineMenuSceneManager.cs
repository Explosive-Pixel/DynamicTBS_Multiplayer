using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineMenuSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject onlineMenuCanvas;
    [SerializeField] private GameObject onlineClientCanvas;

    private List<GameObject> canvasList = new List<GameObject>();

    private void Awake()
    {
        SetCanvasList();
        GoToOnlineMenu();
    }

    private void SetCanvasList()
    {
        canvasList.Add(onlineMenuCanvas);
        canvasList.Add(onlineClientCanvas);
    }

    private void HandleMenus(GameObject menuCanvas)
    {
        foreach (GameObject gameObject in canvasList)
        {
            if (gameObject == menuCanvas)
                menuCanvas.SetActive(true);
            else
                gameObject.SetActive(false);
        }
    }

    public void GoToOnlineMenu()
    {
        HandleMenus(onlineMenuCanvas);
    }
}