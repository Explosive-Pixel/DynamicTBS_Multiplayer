using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoreSceneManager : MonoBehaviour
{
    private List<GameObject> canvasList = new List<GameObject>();

    public GameObject introductionCanvas;
    public GameObject chapter1Canvas;
    public GameObject chapter2Canvas;
    public GameObject chapter3Canvas;
    public GameObject chapter4Canvas;
    public GameObject chapter5Canvas;
    public GameObject chapter6Canvas;
    public GameObject chapter7Canvas;

    private void Awake()
    {
        SetCanvasList();
        GoToCanvas(introductionCanvas);
    }

    private void SetCanvasList()
    {
        canvasList.Add(introductionCanvas);
        canvasList.Add(chapter1Canvas);
        canvasList.Add(chapter2Canvas);
        canvasList.Add(chapter3Canvas);
        canvasList.Add(chapter4Canvas);
        canvasList.Add(chapter5Canvas);
        canvasList.Add(chapter6Canvas);
        canvasList.Add(chapter7Canvas);
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

    public void GoToChapter1Canvas()
    {
        GoToCanvas(chapter1Canvas);
    }

    public void GoToChapter2Canvas()
    {
        GoToCanvas(chapter2Canvas);
    }

    public void GoToChapter3Canvas()
    {
        GoToCanvas(chapter3Canvas);
    }

    public void GoToChapter4Canvas()
    {
        GoToCanvas(chapter4Canvas);
    }

    public void GoToChapter5Canvas()
    {
        GoToCanvas(chapter5Canvas);
    }

    public void GoToChapter6Canvas()
    {
        GoToCanvas(chapter6Canvas);
    }

    public void GoToChapter7Canvas()
    {
        GoToCanvas(chapter7Canvas);
    }
}