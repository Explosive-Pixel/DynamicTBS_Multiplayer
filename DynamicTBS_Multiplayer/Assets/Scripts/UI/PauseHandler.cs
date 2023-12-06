using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    private void Awake()
    {
        GameplayEvents.OnGamePause += TogglePauseCanvas;

        TogglePauseCanvas(false);
    }

    private void TogglePauseCanvas(bool paused)
    {
        gameObject.SetActive(paused);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnGamePause -= TogglePauseCanvas;
    }
}
