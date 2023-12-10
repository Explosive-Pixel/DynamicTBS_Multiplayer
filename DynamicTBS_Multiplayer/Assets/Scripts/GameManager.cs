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
    private static GamePhase currentGamePhase = GamePhase.NONE;

    public static GameType GameType { get { return gameType; } set { gameType = value; } }
    public static GamePhase CurrentGamePhase { get { return currentGamePhase; } }

    private static Dictionary<GamePhase, float> delayAfterGamePhase = new();
    public static Dictionary<GamePhase, float> DelayAfterGamePhase { get { return delayAfterGamePhase; } }

    private void Awake()
    {
        currentGamePhase = GamePhase.NONE;

        delayAfterGamePhase = new Dictionary<GamePhase, float>()
        {
            { GamePhase.DRAFT, delayAfterDraft },
            { GamePhase.PLACEMENT, delayAfterPlacement },
            { GamePhase.GAMEPLAY, delayAfterGameplay }
        };

        GameEvents.OnGamePhaseEnd += ChangeGamePhase;
    }

    private void Start()
    {
        ChangeGamePhase(currentGamePhase);
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