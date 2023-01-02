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
        SubscribeEvents();
        //Wird SpriteManager noch gebraucht? -> Ja für Tiles!
        SpriteManager.LoadSprites();
        PrefabManager.LoadPrefabs();
    }

    public delegate void RecordStart();
    public static event RecordStart OnStartRecording;

    public static void StartRecording()
    {
        if (OnStartRecording != null)
            OnStartRecording();
    }

    #region EventSubscriptions
    private void SubscribeEvents()
    {
        DraftEvents.OnStartDraft += StartRecording;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnStartDraft -= StartRecording;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
    #endregion

    public void EndGame()
    {
        Application.Quit();
    }
}