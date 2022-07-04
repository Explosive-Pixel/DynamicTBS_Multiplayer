using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetRematch : NetMessage
{
    public int teamId;
    public byte wantRematch; // Asking if other player wants a rematch.

    public NetRematch() // Constructing a message.
    {
        Code = OperationCode.REMATCH;
    }

    public NetRematch(DataStreamReader reader) // Receiving a message.
    {
        Code = OperationCode.REMATCH;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(teamId);
        writer.WriteByte(wantRematch);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        teamId = reader.ReadInt();
        wantRematch = reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_REMATCH?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_REMATCH?.Invoke(this, cnn);
    }
}