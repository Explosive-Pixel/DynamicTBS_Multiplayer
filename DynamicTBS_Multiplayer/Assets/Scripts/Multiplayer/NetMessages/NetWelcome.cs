using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class NetWelcome : NetMessage
{
    public int Role { set; get; }
    public int AssignedTeam { set; get; }
    public bool isAdmin;

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
        static int toInt(bool value) { return value == false ? 0 : 1; }

        writer.WriteByte((byte)Code);
        writer.WriteInt(Role);
        writer.WriteInt(AssignedTeam);
        writer.WriteInt(toInt(isAdmin));
    }

    public override void Deserialize(DataStreamReader reader)
    {
        static bool toBool(int value) { return value != 0; }

        // Byte is already read in the NetUtility::OnData.
        Role = reader.ReadInt();
        AssignedTeam = reader.ReadInt();
        isAdmin = toBool(reader.ReadInt());
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