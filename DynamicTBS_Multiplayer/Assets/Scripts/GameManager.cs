using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameType
{
    LOCAL,
    ONLINE
}

public enum GamePhase
{
    DRAFT = 0,
    PLACEMENT = 1,
    GAMEPLAY = 2,
    NONE = 3
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private float delayAfterDraft;
    [SerializeField] private float delayAfterPlacement;
    [SerializeField] private float delayAfterGameplay;

    private static GameType gameType = GameType.LOCAL;
    private static GamePhase currentGamePhase = GamePhase.DRAFT;

    public static GameType GameType { get { return gameType; } set { gameType = value; } }
    public static GamePhase CurrentGamePhase { get { return currentGamePhase; } }

    private readonly Dictionary<GamePhase, float> delayAfterGamePhase = new();

    private void Awake()
    {
        currentGamePhase = GamePhase.DRAFT;

        delayAfterGamePhase.Add(GamePhase.DRAFT, delayAfterDraft);
        delayAfterGamePhase.Add(GamePhase.PLACEMENT, delayAfterPlacement);
        delayAfterGamePhase.Add(GamePhase.GAMEPLAY, delayAfterGameplay);

        GameEvents.OnGamePhaseEnd += ChangeGamePhase;
    }

    public static bool IsPlayer()
    {
        return gameType == GameType.LOCAL || (gameType == GameType.ONLINE && OnlineClient.Instance.UserData.Role == ClientType.PLAYER);
    }

    private void ChangeGamePhase(GamePhase lastGamePhase)
    {
        StartCoroutine(DelayStartNewGamephase(lastGamePhase));
    }

    private IEnumerator DelayStartNewGamephase(GamePhase lastGamePhase)
    {
        yield return new WaitForSeconds(delayAfterGamePhase.ContainsKey(lastGamePhase) ? delayAfterGamePhase[lastGamePhase] : 0);

        currentGamePhase = (GamePhase)(((int)lastGamePhase + 1) % 4);
        GameEvents.StartGamePhase(currentGamePhase);
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseEnd -= ChangeGamePhase;
    }
}