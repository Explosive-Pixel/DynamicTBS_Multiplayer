using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerType draftStartPlayer;
    [SerializeField] private PlayerType placementStartPlayer;
    [SerializeField] private PlayerType gameplayStartPlayer;

    private static PlayerType currentPlayer;
    public static PlayerType CurrentPlayer { get { return currentPlayer; } }
    public static PlayerType ExecutingPlayer { get { return GameManager.GameType == GameType.ONLINE ? Client.Side : CurrentPlayer; } }

    private static Dictionary<GamePhase, PlayerType> startPlayer;
    public static Dictionary<GamePhase, PlayerType> StartPlayer { get { return startPlayer; } }

    private void Awake()
    {
        startPlayer = new Dictionary<GamePhase, PlayerType>()
        {
            { GamePhase.DRAFT, draftStartPlayer },
            { GamePhase.PLACEMENT, placementStartPlayer },
            { GamePhase.GAMEPLAY, gameplayStartPlayer }
        };

        SubscribeEvents();

        currentPlayer = startPlayer[GamePhase.DRAFT];
    }

    public static PlayerType GetOtherSide(PlayerType side)
    {
        if (side == PlayerType.blue)
        {
            return PlayerType.pink;
        }
        return PlayerType.blue;
    }

    public static void NextPlayer()
    {
        GameplayEvents.EndPlayerTurn(CurrentPlayer);

        currentPlayer = GetOtherSide(currentPlayer);
        CurrentPlayerChanged();
    }

    public static bool IsCurrentPlayer(PlayerType side)
    {
        return side == CurrentPlayer;
    }

    public static bool ClientIsCurrentPlayer()
    {
        return ExecutingPlayer == CurrentPlayer;
    }

    private void SetStartPlayer(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.NONE)
            return;

        currentPlayer = startPlayer[gamePhase];
        CurrentPlayerChanged();
    }

    private void ResetCurrentPlayer(GamePhase gamePhase)
    {
        currentPlayer = PlayerType.none;
        CurrentPlayerChanged();
    }

    private static void CurrentPlayerChanged()
    {
        GameplayEvents.ChangeCurrentPlayer(CurrentPlayer);
    }

    #region EventSubscriptions

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += SetStartPlayer;
        GameEvents.OnGamePhaseEnd += ResetCurrentPlayer;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= SetStartPlayer;
        GameEvents.OnGamePhaseEnd -= ResetCurrentPlayer;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}