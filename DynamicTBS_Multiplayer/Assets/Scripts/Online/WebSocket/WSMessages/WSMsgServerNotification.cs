using System;

[Serializable]
public class WSMsgServerNotification : WSMessage
{
    public enum ServerNotification
    {
        LOBBY_NOT_FOUND = 0,
        LOBBY_CREATION_FORBITTEN_MAX_LOBBY_COUNT_REACHED = 1,
        CONNECTION_FORBIDDEN_FULL_LOBBY = 2
    }

    public ServerNotification serverNotification;

    public WSMsgServerNotification()
    {
        code = WSMessageCode.WSMsgServerNotificationCode;
    }

    public override void HandleMessage()
    {
        Client.LeaveLobby();
    }
}
