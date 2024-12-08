using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameType
{
    LOCAL,
    ONLINE
}

public enum GamePhase
{
    NONE = 0,
    DRAFT = 1,
    PLACEMENT = 2,
    GAMEPLAY = 3,
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
            { GamePhase.NONE, 0 },
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
        return gameType == GameType.LOCAL || (gameType == GameType.ONLINE && Client.Role == ClientType.PLAYER);
    }

    public static bool IsSpectator()
    {
        return gameType == GameType.ONLINE && Client.Role == ClientType.SPECTATOR;
    }

    private void ChangeGamePhase(GamePhase lastGamePhase)
    {
        StartCoroutine(DelayStartNewGamephase(lastGamePhase));
    }

    private IEnumerator DelayStartNewGamephase(GamePhase lastGamePhase)
    {
        yield return new WaitForSeconds(GetDelay(lastGamePhase));

        currentGamePhase = (GamePhase)(((int)lastGamePhase + 1) % 4);
        GameEvents.StartGamePhase(currentGamePhase);
    }

    private float GetDelay(GamePhase lastGamePhase)
    {
        if (!delayAfterGamePhase.ContainsKey(lastGamePhase) || (GameType == GameType.ONLINE && Client.IsLoadingGame))
            return 0;

        return delayAfterGamePhase[lastGamePhase];
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseEnd -= ChangeGamePhase;
    }
}