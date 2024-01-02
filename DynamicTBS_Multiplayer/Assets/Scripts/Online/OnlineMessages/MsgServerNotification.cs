using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using static WSMsgServerNotification;

public class MsgServerNotification : OnlineMessage
{
    public ServerNotification serverNotification;
    public GamePhase gamePhase;
    public PlayerType currentPlayer;

    public MsgServerNotification() // Constructing a message.
    {
        Code = OnlineMessageCode.SERVER_NOTIFICATION;
    }

    public MsgServerNotification(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.SERVER_NOTIFICATION;
        Id = reader.ReadFixedString64().Value;
        LobbyId = reader.ReadInt();
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteInt((int)serverNotification);
        writer.WriteInt((int)gamePhase);
        writer.WriteInt((int)currentPlayer);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        serverNotification = (ServerNotification)reader.ReadInt();
        gamePhase = (GamePhase)reader.ReadInt();
        currentPlayer = (PlayerType)reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        switch (serverNotification)
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
            case ServerNotification.TIMEOUT:
                GameplayEvents.TimerTimedOut(gamePhase, currentPlayer);
                break;
        }
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
    }
}
