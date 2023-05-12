using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowAspectRatio : MonoBehaviour
{
    private float aspectRatio = 16f / 9f;

    public int minWidth = 800;
    public int minHeight = 450;

    private GameObject aspectRatioObject;

    private void Awake()
    {
        aspectRatioObject = this.gameObject;
        DontDestroyOnLoad(aspectRatioObject);

        //Screen.SetResolution(minWidth, minHeight, false);
        //SetWindowAspectRatio();
    }

    private void OnGUI()
    {
        //SetWindowAspectRatio();
    }

    private void SetWindowAspectRatio()
    {
        float screenWidth = Screen.currentResolution.width;
        float screenHeight = Screen.currentResolution.height;
        float targetWidth = screenHeight * aspectRatio;
        float targetHeight = screenWidth / aspectRatio;

        if (targetWidth > minWidth && targetHeight > minHeight)
        {
            if (targetWidth < screenWidth)
                Screen.SetResolution((int)targetWidth, (int)screenHeight, false);
            else
                Screen.SetResolution((int)screenWidth, (int)targetHeight, false);
        }
    }
}