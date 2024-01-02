using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public static class Client
{
    private static WSClient ws;
    public static WSClient Ws => ws;

    private static ClientType role;
    public static ClientType Role => role;

    private static string userName;

    private static LobbyId lobbyId;
    public static LobbyId LobbyId => lobbyId;

    private static bool isAdmin;
    public static bool IsAdmin => isAdmin;

    private static PlayerType side = PlayerType.none;
    public static PlayerType Side => side;

    private static bool active = false;
    public static bool Active => active;

    private static ConnectionStatus connectionStatus = ConnectionStatus.UNCONNECTED;
    public static ConnectionStatus ConnectionStatus { get { return connectionStatus; } set { connectionStatus = value; } }

    private static float serverTimeDiff = 0;
    public static float ServerTimeDiff => serverTimeDiff;

    private static bool isLoadingGame = false;
    public static bool IsLoadingGame => isLoadingGame;

    public static void Init(WSClient ws, ClientType role, string userName, LobbyId lobbyId, bool createLobby)
    {
        Client.ws = ws;
        Client.role = role;
        Client.userName = userName;
        Client.lobbyId = lobbyId;

        Client.active = true;
        Client.connectionStatus = ConnectionStatus.CONNECTED;
        Client.side = PlayerType.none;
        Client.isLoadingGame = false;

        TryJoinLobby(createLobby);
    }

    public static bool ShouldReadMessage(PlayerType playerType)
    {
        return side != playerType || isLoadingGame;
    }

    public static bool ShouldSendMessage(PlayerType playerType)
    {
        return side == playerType && !isLoadingGame;
    }

    public static bool AdminShouldSendMessage()
    {
        return isAdmin && !isLoadingGame;
    }

    public static void TryJoinLobby(bool create)
    {
        SendToServer(new WSMsgJoinLobby()
        {
            create = create,
            lobbyName = create ? lobbyId.Name : lobbyId.FullId,
            userName = userName,
            isPlayer = role == ClientType.PLAYER
        });
    }

    public static void EnterLobby(LobbyId lobbyId, bool isAdmin)
    {
        Client.lobbyId = lobbyId;
        Client.isAdmin = isAdmin;
        Client.connectionStatus = ConnectionStatus.IN_LOBBY;
    }

    public static void SyncTimeWithServer(long syncTimestamp)
    {
        var serverTime = TimerUtils.UnixTimeStampToDateTime(syncTimestamp);
        serverTimeDiff = TimerUtils.TimeSince(serverTime);
        Debug.Log("Synchronized Time with Server. Difference is: " + Client.serverTimeDiff);
    }

    public static void ToggleIsLoadingGame()
    {
        isLoadingGame = !isLoadingGame;
        GameEvents.IsGameLoading(isLoadingGame);
    }

    public static void SendStartGameMsg(float draftAndPlacementTimeInSeconds, float gameplayTimeInSeconds, MapType selectedMap, PlayerType adminSide)
    {
        if (Client.isAdmin)
        {
            SendToServer(new WSMsgStartGame()
            {
                draftAndPlacementTimeInSeconds = draftAndPlacementTimeInSeconds,
                gameplayTimeInSeconds = gameplayTimeInSeconds,
                mapType = selectedMap,
                adminSide = adminSide
            });
        }
    }

    public static void StartGame(float draftAndPlacementTimeInSeconds, float gameplayTimeInSeconds, MapType selectedMap, PlayerType adminSide)
    {
        Board.selectedMapType = selectedMap;
        TimerConfig.Init(draftAndPlacementTimeInSeconds, gameplayTimeInSeconds);
        Client.side = Client.role == ClientType.PLAYER ? (Client.isAdmin ? adminSide : PlayerManager.GetOtherSide(adminSide)) : PlayerType.none;
        GameManager.GameType = GameType.ONLINE;

        GameEvents.StartGame();
    }

    public static void Reset()
    {
        active = false;
        connectionStatus = ConnectionStatus.UNCONNECTED;
        ws = null;
        lobbyId = null;
        serverTimeDiff = 0;
    }

    public static void SendToServer(WSMessage wSMessage)
    {
        wSMessage.lobbyId = Client.lobbyId.Id;
        ws.SendMessage(wSMessage);
    }
}
