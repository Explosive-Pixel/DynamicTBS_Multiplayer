using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class NetExecuteUIAction : NetMessage
{
    // When transporting information through the internet it's important to choose the most basic types possible.
    public int uiActionType;
    public int playerId;

    public NetExecuteUIAction() // Constructing a message.
    {
        Code = OperationCode.EXECUTE_UIACTION;
    }

    public NetExecuteUIAction(DataStreamReader reader) // Receiving a message.
    {
        Code = OperationCode.EXECUTE_UIACTION;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(uiActionType);
        writer.WriteInt(playerId);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        uiActionType = reader.ReadInt();
        playerId = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_EXECUTE_UIACTION?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_EXECUTE_UIACTION?.Invoke(this, cnn);
    }
}
