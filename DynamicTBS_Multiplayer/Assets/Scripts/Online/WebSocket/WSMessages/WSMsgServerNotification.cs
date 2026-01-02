using System;

[Serializable]
public class WSMsgServerNotification : WSMessage
{
    public enum ServerNotification
    {
        LOBBY_NOT_FOUND = 0,
        LOBBY_CREATION_FORBITTEN_MAX_LOBBY_COUNT_REACHED = 1,
        CONNECTION_FORBIDDEN_FULL_LOBBY = 2,
        TOGGLE_LOAD_GAME_STATUS = 3,
        TIMEOUT = 4
    }

    public ServerNotification serverNotification;

    public WSMsgServerNotification()
    {
        code = WSMessageCode.WSMsgServerNotificationCode;
    }

    public override void HandleMessage()
    {
        switch (serverNotification)
        {
            case ServerNotification.TOGGLE_LOAD_GAME_STATUS:
                Client.ToggleIsLoadingGame();
                break;
            case ServerNotification.TIMEOUT:
                GameplayEvents.TimerTimedOut(GameManager.CurrentGamePhase, PlayerManager.CurrentPlayer);
                break;
        }
    }
}
