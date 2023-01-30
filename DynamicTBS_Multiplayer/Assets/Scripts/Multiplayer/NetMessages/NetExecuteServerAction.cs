using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetExecuteServerAction : NetMessage
{
    // When transporting information through the internet it's important to choose the most basic types possible.
    public int serverActionType;

    public NetExecuteServerAction() // Constructing a message.
    {
        Code = OperationCode.EXECUTE_SERVER_ACTION;
    }

    public NetExecuteServerAction(DataStreamReader reader) // Receiving a message.
    {
        Code = OperationCode.EXECUTE_SERVER_ACTION;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(serverActionType);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        serverActionType = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_EXECUTE_SERVER_ACTION?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_EXECUTE_SERVER_ACTION?.Invoke(this, cnn);
    }
}
