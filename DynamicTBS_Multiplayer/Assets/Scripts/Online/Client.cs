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
    ATTEMPT_CONNECTION = 1,
    CONNECTED = 2,
    LOBBY_NOT_FOUND = 3,
    CONNECTION_DECLINED = 4,
    IN_LOBBY = 5
}

public static class Client
{
    public static string Uuid { get; private set; } = Guid.NewGuid().ToString();

    public static ClientType Role { get; set; }
    public static Lobby CurrentLobby { get; private set; }

    public static bool Active { get; set; } = false;
    public static ConnectionStatus ConnectionStatus { get; set; } = ConnectionStatus.UNCONNECTED;
    public static float ServerTimeDiff { get; private set; } = 0;

    public static bool IsLoadingGame { get; private set; } = false;

    public static bool IsReady { get; set; } = false;

    public static bool IsAdmin { get { return CurrentLobby != null && CurrentLobby.Admin != null && CurrentLobby.Admin.uuid == Uuid; } }

    public static ClientInfo ClientInfo
    {
        get
        {
            return new ClientInfo()
            {
                uuid = Uuid,
                name = PlayerSetup.Name,
                isPlayer = Role == ClientType.PLAYER,
                side = PlayerSetup.Side,
                isReady = IsReady
            };
        }
        private set
        {
            if (value.uuid == Uuid)
            {
                PlayerSetup.SetupSide(value.side);
                Client.IsReady = value.isReady;
            }
        }
    }

    public static bool IsConnectedToServer { get { return ConnectionStatus >= ConnectionStatus.CONNECTED; } }
    public static bool InLobby { get { return CurrentLobby != null; } }

    public static bool ShouldReadMessage(PlayerType playerType)
    {
        return PlayerSetup.Side != playerType || IsLoadingGame;
    }

    public static bool ShouldSendMessage(PlayerType playerType)
    {
        return PlayerSetup.Side == playerType && !IsLoadingGame;
    }

    public static void CreateLobby(string lobbyName, bool isPrivateLobby)
    {
        SendToServer(new WSMsgCreateLobby()
        {
            lobbyName = lobbyName,
            isPrivateLobby = isPrivateLobby,
            clientInfo = ClientInfo,
            gameConfig = GameSetup.GameConfig
        });
    }

    public static void ConfigLobby()
    {
        SendToServer(new WSMsgConfigLobby()
        {
            clientInfo = ClientInfo,
            gameConfig = GameSetup.GameConfig
        });
    }

    public static void JoinLobby(string lobbyFullName, ClientType role)
    {
        Client.Role = role;

        SendToServer(new WSMsgJoinLobby()
        {
            lobbyFullName = lobbyFullName,
            clientInfo = ClientInfo,
            isReconnect = false
        });
    }

    public static void UpdateLobby(LobbyInfo lobbyInfo)
    {
        CurrentLobby = new Lobby(lobbyInfo);
        GameSetup.Setup(lobbyInfo.gameConfig);

        ClientInfo = CurrentLobby.GetClientInfo(Uuid);

        ConnectionStatus = ConnectionStatus.IN_LOBBY;
        MenuEvents.UpdateCurrentLobby();
    }

    public static void Reconnect()
    {
        if (!InLobby)
            return;

        SendToServer(new WSMsgJoinLobby()
        {
            lobbyFullName = CurrentLobby.LobbyId.FullId,
            clientInfo = ClientInfo,
            isReconnect = true
        });
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


    public static void Reset()
    {
        ConnectionStatus = ConnectionStatus.UNCONNECTED;
        CurrentLobby = null;
        ServerTimeDiff = 0;
        IsReady = false;
    }

    public static void SendToServer(WSMessage wSMessage)
    {
        if (IsLoadingGame)
            return;

        wSMessage.lobbyId = InLobby ? CurrentLobby.LobbyId.Id : 0;

        WSClient.Instance.SendMessage(wSMessage);
    }
}
