using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialCanvas;

    private void Awake()
    {
        tutorialCanvas.SetActive(true);
    }
}