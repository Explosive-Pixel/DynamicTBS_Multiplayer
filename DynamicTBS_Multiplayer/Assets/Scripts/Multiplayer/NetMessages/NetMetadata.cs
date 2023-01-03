using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class NetMetadata : NetMessage
{
    public int playerCount;
    public int spectatorCount;

    public NetMetadata() // Constructing a message.
    {
        Code = OperationCode.METADATA;
    }

    public NetMetadata(DataStreamReader reader) // Receiving a message.
    {
        Code = OperationCode.METADATA;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(playerCount);
        writer.WriteInt(spectatorCount);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        playerCount = reader.ReadInt();
        spectatorCount = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_METADATA?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_METADATA?.Invoke(this, cnn);
    }
}