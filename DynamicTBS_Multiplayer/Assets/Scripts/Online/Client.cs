using UnityEngine;
using System;

public enum ClientType
{
    PLAYER = 1,
    SPECTATOR = 2
}

public enum ConnectionStatus
{
    UNCONNECTED = 0,
    CONNECTED = 1,
    LOBBY_NOT_FOUND = 2,
    CONNECTION_DECLINED = 3,
    ATTEMPT_CONNECTION = 4,
    IN_LOBBY = 5
}

public static class Client
{
    private static string Uuid { get; set; }
    private static string UserName { get; set; }

    public static ClientType Role { get; private set; }
    public static LobbyId LobbyId { get; private set; }
    public static bool IsAdmin { get; private set; }
    public static PlayerType Side { get; private set; } = PlayerType.none;
    public static bool Active { get; private set; } = false;
    public static ConnectionStatus ConnectionStatus { get; set; } = ConnectionStatus.UNCONNECTED;
    public static float ServerTimeDiff { get; private set; } = 0;
    public static bool IsLoadingGame { get; private set; } = false;

    public static void Init(ClientType role, string userName, LobbyId lobbyId)
    {
        Client.Uuid = Guid.NewGuid().ToString();
        Client.Role = role;
        Client.UserName = userName;
        Client.LobbyId = lobbyId;

        Client.Side = PlayerType.none;
        Client.IsLoadingGame = false;
    }

    public static bool ShouldReadMessage(PlayerType playerType)
    {
        return Side != playerType || IsLoadingGame;
    }

    public static bool ShouldSendMessage(PlayerType playerType)
    {
        return Side == playerType && !IsLoadingGame;
    }

    public static bool AdminShouldSendMessage()
    {
        return IsAdmin && !IsLoadingGame;
    }

    public static void TryJoinLobby(bool create, bool isReconnect)
    {
        Client.Active = true;
        //Client.ConnectionStatus = ConnectionStatus.CONNECTED;

        SendToServer(new WSMsgJoinLobby()
        {
            create = create,
            lobbyName = create ? LobbyId.Name : LobbyId.FullId,
            clientUUID = Uuid,
            userName = UserName,
            isPlayer = Role == ClientType.PLAYER,
            isReconnect = isReconnect
        });
    }

    public static void EnterLobby(LobbyId lobbyId, bool isAdmin)
    {
        Client.LobbyId = lobbyId;
        Client.IsAdmin = isAdmin;
        Client.ConnectionStatus = ConnectionStatus.IN_LOBBY;
    }

    public static void SyncTimeWithServer(long syncTimestamp)
    {
        var serverTime = TimerUtils.UnixTimeStampToDateTime(syncTimestamp);
        ServerTimeDiff = TimerUtils.TimeSince(serverTime);
        Debug.Log("Synchronized Time with Server. Difference is: " + Client.ServerTimeDiff);
    }

    public static void ToggleIsLoadingGame()
    {
        IsLoadingGame = !IsLoadingGame;
        GameEvents.IsGameLoading(IsLoadingGame);
    }

    public static void SendStartGameMsg(float draftAndPlacementTimeInSeconds, float gameplayTimeInSeconds, MapType selectedMap, PlayerType adminSide)
    {
        if (Client.IsAdmin)
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
        Client.Side = Client.Role == ClientType.PLAYER ? (Client.IsAdmin ? adminSide : PlayerManager.GetOtherSide(adminSide)) : PlayerType.none;
        GameManager.GameType = GameType.ONLINE;

        GameEvents.StartGame();
    }

    public static void Reset()
    {
        Active = false;
        ConnectionStatus = ConnectionStatus.UNCONNECTED;
        LobbyId = null;
        ServerTimeDiff = 0;
    }

    public static void SendToServer(WSMessage wSMessage)
    {
        wSMessage.lobbyId = Client.LobbyId.Id;

        WSClient.Instance.SendMessage(wSMessage);
    }
}
