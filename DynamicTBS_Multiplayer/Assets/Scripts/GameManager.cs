using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameType gameType = GameType.local;
    private static bool hasGameStarted;

    private void Awake()
    {
        SubscribeEvents();
        //Wird SpriteManager noch gebraucht?
        SpriteManager.LoadSprites();
        PrefabManager.LoadPrefabs();
        hasGameStarted = false;
    }

    public delegate void RecordStart();
    public static event RecordStart OnStartRecording;

    public static void StartRecording()
    {
        if (OnStartRecording != null)
            OnStartRecording();
    }

    public static bool HasGameStarted()
    {
        return hasGameStarted;
    }

    private static void SetGameStarted()
    {
        hasGameStarted = true;
    }

    #region EventSubscriptions
    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += SetGameStarted;
        DraftEvents.OnStartDraft += StartRecording;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= SetGameStarted;
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