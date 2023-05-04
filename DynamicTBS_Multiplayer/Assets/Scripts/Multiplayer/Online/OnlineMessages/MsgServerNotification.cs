using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public enum ServerNotification
{
    LOBBY_NOT_FOUND = 1,
    CONNECTION_FORBIDDEN_FULL_LOBBY = 2,
    TOGGLE_LOAD_GAME_STATUS = 3
}

public class MsgServerNotification : OnlineMessage
{
    public ServerNotification serverNotification;

    public MsgServerNotification() // Constructing a message.
    {
        Code = OnlineMessageCode.SERVER_NOTIFICATION;
    }

    public MsgServerNotification(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.SERVER_NOTIFICATION;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteInt((int)serverNotification);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        LobbyId = reader.ReadInt();
        serverNotification = (ServerNotification)reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        switch(serverNotification)
        {
            case ServerNotification.LOBBY_NOT_FOUND:
                OnlineClient.Instance.ConnectionStatus = ConnectionStatus.LOBBY_NOT_FOUND;
                break;
            case ServerNotification.CONNECTION_FORBIDDEN_FULL_LOBBY:
                OnlineClient.Instance.ConnectionStatus = ConnectionStatus.CONNECTION_DECLINED;
                break;
            case ServerNotification.TOGGLE_LOAD_GAME_STATUS:
                OnlineClient.Instance.ToggleIsLoadingGame();
                break;
        }
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
    }
}
