using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

// Message with metadata used to keep connection alive
public class MsgMetadata : OnlineMessage
{
    public int playerCount;
    public int spectatorCount;

    public MsgMetadata() // Constructing a message.
    {
        Code = OnlineMessageCode.METADATA;
    }

    public MsgMetadata(DataStreamReader reader) // Receiving a message.
    {
        Code = OnlineMessageCode.METADATA;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer, int lobbyId)
    {
        base.Serialize(ref writer, lobbyId);
        writer.WriteInt(playerCount);
        writer.WriteInt(spectatorCount);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        LobbyId = reader.ReadInt();
        playerCount = reader.ReadInt();
        spectatorCount = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        OnlineClient.Instance.SendToServer(this); // Sends message back to keep both sides alive.

        OnlineClient.Instance.PlayerCount = playerCount;
        OnlineClient.Instance.SpectatorCount = spectatorCount;
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
    }
}
