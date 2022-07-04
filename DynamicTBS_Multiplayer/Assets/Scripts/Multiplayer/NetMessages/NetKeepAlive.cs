using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetKeepAlive : NetMessage
{
    public NetKeepAlive() // Constructing a message.
    {
        Code = OperationCode.KEEP_ALIVE;
    }

    public NetKeepAlive(DataStreamReader reader) // Receiving a message.
    {
        Code = OperationCode.KEEP_ALIVE;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        // Since the operation code byte has already been read and it's a keep alive message, there's nothing else to read here.
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_KEEP_ALIVE?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_KEEP_ALIVE?.Invoke(this, cnn);
    }
}