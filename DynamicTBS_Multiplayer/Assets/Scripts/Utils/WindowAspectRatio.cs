using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WindowAspectRatio : MonoBehaviour
{
    private static WindowAspectRatio Instance { set; get; }

    public float targetAspectRatio = 16f / 9f;
    private List<Canvas> canvases;

    private int currentWith;
    private int currentHeight;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitCanvases();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (currentWith == Screen.width && currentHeight == Screen.height)
            return;

        UpdateResolution();
    }

    private void UpdateResolution()
    {
        SetCurrentResolution();

        SetCameraAspectRatio();

        foreach (Canvas canvas in canvases)
        {
            if (canvas == null)
                return;

            SetCanvasAspectRatio(canvas);
        }
    }

    private void SetCameraAspectRatio()
    {
        float windowaspect = (float)Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetAspectRatio;
        Rect rect = Camera.main.rect;

        if (scaleheight < 1f)
        {
            rect.width = 1f;
            rect.height = scaleheight;
            rect.x = 0f;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = 1f / scaleheight;
            rect.height = 1f;
            rect.x = (1f - 1f / scaleheight) / 2f;
            rect.y = 0f;
        }

        Camera.main.rect = rect;
    }

    private void SetCanvasAspectRatio(Canvas canvas)
    {
        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
        float screenRatio = Screen.width / (float)Screen.height;
        float referenceRatio = canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y;

        if (screenRatio >= referenceRatio)
        {
            canvasScaler.matchWidthOrHeight = 1f;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 0f;
        }
    }

    private void InitCanvases()
    {
        canvases = new List<Canvas>(FindObjectsOfType<Canvas>(true));
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        InitCanvases();
        UpdateResolution();
    }

    private void SetCurrentResolution()
    {
        currentWith = Screen.width;
        currentHeight = Screen.height;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}