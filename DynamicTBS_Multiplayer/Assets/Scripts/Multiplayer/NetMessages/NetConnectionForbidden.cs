using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetConnectionForbidden : NetMessage
{
    public NetConnectionForbidden() // Constructing a message.
    {
        Code = OperationCode.CONNECTION_FORBIDDEN;
    }

    public NetConnectionForbidden(DataStreamReader reader) // Receiving a message.
    {
        Code = OperationCode.CONNECTION_FORBIDDEN;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }

    public override void Deserialize(DataStreamReader reader)
    {
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_CONNECTION_FORBIDDEN?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_CONNECTION_FORBIDDEN?.Invoke(this, cnn);
    }
}