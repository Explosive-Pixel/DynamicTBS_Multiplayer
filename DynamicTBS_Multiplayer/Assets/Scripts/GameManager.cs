using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameType gameType = GameType.local;

    private void Awake()
    {
        //Wird SpriteManager noch gebraucht? -> Ja für Tiles!
        SpriteManager.LoadSprites();
        PrefabManager.LoadPrefabs();
    }

    public void EndGame()
    {
        Application.Quit();
    }
}