using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject startMenuCanvas;
    [SerializeField] private GameObject onlineMenuCanvas;
    [SerializeField] private GameObject draftCanvas;
    [SerializeField] private GameObject gameplayCanvas;

    private void Awake()
    {
        SpriteManager.LoadSprites();
    }

    public void GoToStartMenu()
    {
        startMenuCanvas.SetActive(true);
        onlineMenuCanvas.SetActive(false);
        draftCanvas.SetActive(false);
        gameplayCanvas.SetActive(false);
    }

    public void GoToOnlineMenu()
    {
        startMenuCanvas.SetActive(false);
        onlineMenuCanvas.SetActive(true);
        draftCanvas.SetActive(false);
        gameplayCanvas.SetActive(false);
    }

    public void GotToDraftScreen()
    {
        startMenuCanvas.SetActive(false);
        onlineMenuCanvas.SetActive(false);
        draftCanvas.SetActive(true);
        gameplayCanvas.SetActive(false);
    }

    public void GoToGameplayScreen()
    {
        startMenuCanvas.SetActive(false);
        onlineMenuCanvas.SetActive(false);
        draftCanvas.SetActive(false);
        gameplayCanvas.SetActive(true);
    }
}