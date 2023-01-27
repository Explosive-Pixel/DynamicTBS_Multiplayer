using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject startMenuCanvas;

    private void Awake()
    {
        startMenuCanvas.SetActive(true);
    }

    private void Start()
    {
        AudioEvents.EnteringMainMenu();
    }
}