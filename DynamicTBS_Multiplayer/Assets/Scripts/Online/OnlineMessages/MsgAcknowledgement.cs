using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class MsgAcknowledgement : OnlineMessage
{
    public string msgId;

    public MsgAcknowledgement() // Constructing a message.
    {
        Code = OnlineMessageCode.ACKNOWLEDGE_MSG;
    }

    public MsgAcknowledgement(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.ACKNOWLEDGE_MSG;
        Id = reader.ReadFixedString64().Value;
        LobbyId = reader.ReadInt();
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteFixedString64(msgId);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        msgId = reader.ReadFixedString64().Value;
    }

    public override void ReceivedOnClient()
    {
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
    }
}
