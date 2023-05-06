using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public enum UIAction
{
    PAUSE_GAME = 1,
    UNPAUSE_GAME = 2,
    OFFER_DRAW = 3,
    ACCEPT_DRAW = 4,
    DECLINE_DRAW = 5,
    SURRENDER = 6
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
            GameplayEvents.UIActionExecuted(PlayerManager.GetPlayer(playerId), uiAction);
        }
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        OnlineServer.Instance.Broadcast(this, LobbyId);

        if (uiAction == UIAction.PAUSE_GAME || uiAction == UIAction.UNPAUSE_GAME)
        {
            OnlineServer.Instance.PauseGame(LobbyId, uiAction);
        }
    }
}
