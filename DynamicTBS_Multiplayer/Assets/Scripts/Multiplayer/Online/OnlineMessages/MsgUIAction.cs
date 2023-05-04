using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public enum UIAction
{
    START_GAME = 1,
    PAUSE_GAME = 2,
    UNPAUSE_GAME = 3,
    OFFER_DRAW = 4,
    ACCEPT_DRAW = 5,
    DECLINE_DRAW = 6,
    SURRENDER = 7,
    OFFER_REMATCH = 8,
    ACCEPT_REMATCH = 9,
    DECLINE_REMATCH = 10,
    SKIP_TURN = 11
}

public class MsgUIAction : OnlineMessage
{
    public PlayerType playerId;
    public UIAction uiAction;

    public MsgUIAction() // Constructing a message.
    {
        Code = OnlineMessageCode.UI_ACTION;
    }

    public MsgUIAction(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.UI_ACTION;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteByte((byte)playerId);
        writer.WriteByte((byte)uiAction);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        LobbyId = reader.ReadInt();
        playerId = (PlayerType)reader.ReadByte();
        uiAction = (UIAction)reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        if (OnlineClient.Instance.ShouldReadMessage(playerId))
        {
            switch(uiAction)
            {
                case UIAction.START_GAME:
                    OnlineClient.Instance.StartGame();
                    break;
                default:
                    GameplayEvents.UIActionExecuted(PlayerManager.GetPlayer(playerId), uiAction);
                    break;
            }
        }
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        OnlineServer.Instance.Broadcast(this, LobbyId);
    }
}
