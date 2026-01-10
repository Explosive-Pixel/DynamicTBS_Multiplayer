using UnityEngine;
using System;
using System.Collections.Generic;

public enum ClientType
{
    PLAYER = 1,
    SPECTATOR = 2
}

public static class Client
{
    public static string Uuid { get; private set; } = Guid.NewGuid().ToString();

    public static ClientType Role { get; set; }
    public static Lobby CurrentLobby { get; private set; }

    public static ConnectionState ConnectionStatus { get { return WSClient.Instance.ConnectionState; } }
    public static float ServerTimeDiff { get; private set; } = 0;

    public static bool IsLoadingGame { get; private set; } = false;

    public static bool IsReady { get; set; } = false;

    public static bool IsAdmin { get { return CurrentLobby != null && CurrentLobby.Admin != null && CurrentLobby.Admin.uuid == Uuid; } }

    public static bool IsWaitingForActionExecution { get; set; }

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

    public static bool IsConnectedToServer { get { return ConnectionStatus == ConnectionState.CONNECTED || ConnectionStatus == ConnectionState.INSTABLE; } }
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

        MenuEvents.UpdateCurrentLobby();
    }

    public static void LeaveLobby()
    {
        CurrentLobby = null;
    }

    public static async void Reconnect()
    {
        if (!InLobby)
            return;

        await WSClient.Instance.SendDirectly(new WSMsgJoinLobby()
        {
            lobbyFullName = CurrentLobby.LobbyId.FullId,
            lobbyId = 0,
            clientInfo = ClientInfo,
            isReconnect = true
        });
    }

    public static void LoadGame(List<WSMessage> msgHistory)
    {
        ToggleIsLoadingGame();
        WSClient.Instance.LoadGame(msgHistory);
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
        CurrentLobby = null;
        ServerTimeDiff = 0;
        IsReady = false;
    }

    public static void SendToServer(WSMessage wSMessage)
    {
        if (IsLoadingGame)
            return;

        wSMessage.lobbyId = InLobby ? CurrentLobby.LobbyId.Id : 0;


        if (wSMessage.code == WSMessageCode.WSMsgDraftCharacterCode || wSMessage.code == WSMessageCode.WSMsgPerformActionCode)
        {
            IsWaitingForActionExecution = true;
        }

        WSClient.Instance.SendMessage(wSMessage);
    }
}
