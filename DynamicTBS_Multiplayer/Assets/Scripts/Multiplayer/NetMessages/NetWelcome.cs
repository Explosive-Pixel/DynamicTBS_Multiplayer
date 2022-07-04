using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetWelcome : NetMessage
{
    public int AssignedTeam { set; get; }

    public NetWelcome() // Constructing a message.
    {
        Code = OperationCode.WELCOME;
    }

    public NetWelcome(DataStreamReader reader) // Receiving a message.
    {
        Code = OperationCode.WELCOME;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(AssignedTeam);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        // Byte is already read in the NetUtility::OnData.
        AssignedTeam = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_WELCOME?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_WELCOME?.Invoke(this, cnn);
    }
}